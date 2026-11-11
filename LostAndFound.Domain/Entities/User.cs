using LostAndFound.Domain.Enums;
using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Domain.Entities;

public class User
{
    public int Id { get; private set; }

    public string FullName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    private User(
        string fullName,
        string email,
        string passwordHash)
    {
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = UserRole.User;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(
        string fullName,
        string email,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        return new User(
            fullName,
            email,
            passwordHash);
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        PasswordHash = passwordHash;
    }

    public void ChangeRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
            throw new DomainException("Invalid user role.");

        Role = role;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}