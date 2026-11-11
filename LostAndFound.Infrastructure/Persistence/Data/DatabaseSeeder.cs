using LostAndFound.Application.Features.Auth.Interfaces;
using LostAndFound.Domain.Entities;
using LostAndFound.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LostAndFound.Infrastructure.Persistence.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAdminAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var email = configuration["AdminSeed:Email"];
        var password = configuration["AdminSeed:Password"];
        var fullName = configuration["AdminSeed:FullName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidOperationException(
                "Admin seed configuration is missing.");
        }

        using var scope = serviceProvider.CreateScope();

        var userRepository = scope.ServiceProvider
            .GetRequiredService<IUserRepository>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher>();

        email = email.Trim().ToLowerInvariant();

        if (await userRepository.EmailExistsAsync(email))
            return;

        var passwordHash = passwordHasher.HashPassword(password);

        var admin = User.Create(
            fullName,
            email,
            passwordHash);

        admin.ChangeRole(UserRole.Admin);

        await userRepository.AddAsync(admin);
        await userRepository.SaveChangesAsync();
    }
}