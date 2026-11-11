using LostAndFound.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LostAndFound.Infrastructure.Persistence.Configurations;

public class ReportImageConfiguration
    : IEntityTypeConfiguration<ReportImage>
{
    public void Configure(
        EntityTypeBuilder<ReportImage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.Report)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}