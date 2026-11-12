using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Features.Auth.Interfaces;
using LostAndFound.Application.Features.Categories.Interfaces;
using LostAndFound.Application.Features.Categories.Services;
using LostAndFound.Application.Features.Locations.Interfaces;
using LostAndFound.Application.Features.Locations.Services;
using LostAndFound.Application.Features.Notifications.Interfaces;
using LostAndFound.Application.Features.Reports.Interfaces;
using LostAndFound.Application.Features.Reports.Services;
using LostAndFound.Application.Messaging;
using LostAndFound.Infrastructure.Common.Services;
using LostAndFound.Infrastructure.Features.Auth.Repositories;
using LostAndFound.Infrastructure.Features.Auth.Security;
using LostAndFound.Infrastructure.Features.Notifications;
using LostAndFound.Infrastructure.Messaging;
using LostAndFound.Infrastructure.Options;
using LostAndFound.Infrastructure.Persistence.Data;
using LostAndFound.Infrastructure.Persistence.Repositories;
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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<ILocationRepository, LocationRepository>();

        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IReportRepository, ReportRepository>();

        services.AddSingleton<SimilarityCalculator>();
        services.AddScoped<IReportMatchingService, ReportMatchingService>();

        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddHostedService<NotificationConsumer>();

        return services;
    }
}