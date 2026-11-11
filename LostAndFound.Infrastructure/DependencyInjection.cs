using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Features.Auth.Login.Interfaces;
using LostAndFound.Application.Features.Auth.Register.Interfaces;
using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Application.Features.Categories.Services;
using LostAndFound.Application.Features.Locations.Interfaces;
using LostAndFound.Application.Features.Locations.Services;
using LostAndFound.Application.Features.Notifications.Interfaces;
using LostAndFound.Application.Features.Reports.Interfaces;
using LostAndFound.Application.Features.Reports.Services;
using LostAndFound.Application.Messaging;
using LostAndFound.Infrastructure.Common.Services;
using LostAndFound.Infrastructure.Features.Auth.Login;
using LostAndFound.Infrastructure.Features.Auth.Register;
using LostAndFound.Infrastructure.Features.Notifications;
using LostAndFound.Infrastructure.Identity;
using LostAndFound.Infrastructure.Messaging;
using LostAndFound.Infrastructure.Persistence.Data;
using LostAndFound.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LostAndFound.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Identity
        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        // Authentication
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Current user
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Categories
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

        // Locations
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ILocationRepository, LocationRepository>();

        // Reports
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IReportRepository, ReportRepository>();

        // Notifications
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // RabbitMQ
        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddHostedService<NotificationConsumer>();

        return services;
    }
}