using FluentValidation;
using LostAndFound.Application;
using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Features.Auth.Register.Validators;
using LostAndFound.Infrastructure;
using LostAndFound.Infrastructure.Identity;
using LostAndFound.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

        // Image storage service
        builder.Services.AddScoped<
            IImageStorageService,
            LocalImageStorageService>();

        // Authentication - JWT
        builder.Services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtOptions =
                    builder.Configuration
                        .GetSection(JwtOptions.SectionName)
                        .Get<JwtOptions>()!;

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

        // Make admin@gmail.com an Admin
        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider
                    .GetRequiredService<
                        RoleManager<IdentityRole<int>>>();

            var userManager =
                scope.ServiceProvider
                    .GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<int>("Admin"));
            }

            var user =
                await userManager.FindByEmailAsync(
                    "admin@gmail.com");

            if (user != null &&
                !await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(
                    user,
                    "Admin");
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}