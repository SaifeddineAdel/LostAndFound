using FluentValidation;
using LostAndFound.Application;
using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Features.Auth.Validators;
using LostAndFound.Infrastructure;
using LostAndFound.Infrastructure.Options;
using LostAndFound.Infrastructure.Persistence.Data;
using LostAndFound.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace LostAndFound.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

        builder.Services.AddOpenApi();

        builder.Services.AddValidatorsFromAssemblyContaining<
            RegisterRequestValidator>();

        builder.Services.AddInfrastructure(
            builder.Configuration);

        builder.Services.AddApplication();

        builder.Services.AddScoped<
            IImageStorageService,
            LocalImageStorageService>();

        var jwtOptions = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey) ||
            Encoding.UTF8.GetByteCount(jwtOptions.SecretKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 bytes.");
        }

        builder.Services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtOptions.SecretKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (app.Environment.IsEnvironment("Docker"))
        {
            await using var scope =
                app.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.MigrateAsync();
        }

        await DatabaseSeeder.SeedAdminAsync(
            app.Services,
            app.Configuration);

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (!app.Environment.IsEnvironment("Docker"))
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}