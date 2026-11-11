using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Domain.Entities;

public class RefreshToken
{
    public int Id { get; private set; }

    public int UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public User User { get; private set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken()
    {
    }

    private RefreshToken(
        int userId,
        string tokenHash,
        DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public static RefreshToken Create(
        int userId,
        string tokenHash,
        DateTime expiresAt)
    {
        if (userId <= 0)
            throw new DomainException("Invalid user ID.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("Token hash is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Expiration date must be in the future.");

        return new RefreshToken(
            userId,
            tokenHash,
            expiresAt);
    }

    public void Revoke()
    {
        if (!IsActive)
            throw new DomainException("Refresh token is not active.");

        RevokedAt = DateTime.UtcNow;
    }
}