using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularTemplate.Common.Infrastructure.Auditing.Configurations;
using ModularTemplate.Modules.Fees.Domain.FeeSchedules;

namespace ModularTemplate.Modules.Fees.Infrastructure.Persistence.Configurations;

internal sealed class FeeScheduleConfiguration : IEntityTypeConfiguration<FeeSchedule>
{
    public void Configure(EntityTypeBuilder<FeeSchedule> builder)
    {
        builder.ToTable("fee_schedules");

        builder.HasKey(fs => fs.Id);

        builder.Property(fs => fs.Id)
            .HasColumnName("id");

        builder.Property(fs => fs.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(fs => fs.FeeCategory)
            .HasColumnName("fee_category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(fs => fs.FeeType)
            .HasColumnName("fee_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(fs => fs.Rate)
            .HasColumnName("rate")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(fs => fs.MinAmount)
            .HasColumnName("min_amount")
            .HasPrecision(18, 2);

        builder.Property(fs => fs.MaxAmount)
            .HasColumnName("max_amount")
            .HasPrecision(18, 2);

        builder.Property(fs => fs.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(fs => fs.EffectiveFrom)
            .HasColumnName("effective_from")
            .IsRequired();

        builder.Property(fs => fs.EffectiveTo)
            .HasColumnName("effective_to");

        // Create index for common query pattern
        builder.HasIndex(fs => new { fs.FeeCategory, fs.IsActive, fs.EffectiveFrom })
            .HasDatabaseName("ix_fee_schedules_category_active_effective");

        // Configure audit fields from IAuditableEntity
        builder.ConfigureAuditProperties();
    }
}
