using LostAndFound.Application.Common.Exceptions;
using LostAndFound.Application.Features.Auth.DTOs;
using LostAndFound.Application.Features.Auth.Interfaces;
using LostAndFound.Domain.Entities;

namespace LostAndFound.Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.EmailExistsAsync(email))
            throw new ConflictException("Email is already registered.");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.Create(
            request.FullName,
            email,
            passwordHash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return await CreateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null ||
            !_passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
            throw new UnauthorizedException("User account is inactive.");

        return await CreateAuthResponseAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new UnauthorizedException("Refresh token is required.");

        var tokenHash = _tokenService.HashRefreshToken(
            request.RefreshToken);

        var storedToken = await _refreshTokenRepository
            .GetByHashAsync(tokenHash);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedException("Invalid refresh token.");

        var user = await _userRepository.GetByIdAsync(
            storedToken.UserId);

        if (user is null || !user.IsActive)
            throw new UnauthorizedException("User account is unavailable.");

        storedToken.Revoke();

        return await CreateAuthResponseAsync(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedException("Refresh token is required.");

        var tokenHash = _tokenService.HashRefreshToken(refreshToken);

        var storedToken = await _refreshTokenRepository
            .GetByHashAsync(tokenHash);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedException("Invalid refresh token.");

        storedToken.Revoke();

        await _refreshTokenRepository.SaveChangesAsync();
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = RefreshToken.Create(
            user.Id,
            _tokenService.HashRefreshToken(refreshToken),
            _tokenService.GetRefreshTokenExpiration());

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = _tokenService.GetAccessTokenExpiration(),
            RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt,
            User = new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            }
        };
    }
}