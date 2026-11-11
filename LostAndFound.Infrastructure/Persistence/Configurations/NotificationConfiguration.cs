using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Report>()
            .WithMany()
            .HasForeignKey(x => x.ReportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.IsRead });
    }
}