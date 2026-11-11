using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Infrastructure.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<SubCategory> SubCategories { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Report> Reports { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}