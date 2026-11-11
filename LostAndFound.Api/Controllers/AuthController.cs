using FluentValidation;
using LostAndFound.Application.Features.Auth.DTOs;
using LostAndFound.Application.Features.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LostAndFound.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var validationResult =
            await _registerValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var response =
            await _authService.RegisterAsync(request);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var validationResult =
            await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var response =
            await _authService.LoginAsync(request);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request)
    {
        var response =
            await _authService.RefreshAsync(request);

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        RefreshTokenRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);

        return NoContent();
    }
}