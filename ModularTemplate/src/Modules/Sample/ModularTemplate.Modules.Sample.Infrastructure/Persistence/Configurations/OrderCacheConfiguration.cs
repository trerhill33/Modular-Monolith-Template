using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularTemplate.Modules.Sample.Domain.OrdersCache;

namespace ModularTemplate.Modules.Sample.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for OrderCache entity.
/// Note: No audit or soft delete fields - cache entities are simple data copies.
/// </summary>
internal sealed class OrderCacheConfiguration : IEntityTypeConfiguration<OrderCache>
{
    public void Configure(EntityTypeBuilder<OrderCache> builder)
    {
        builder.ToTable("orders_cache");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(o => o.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(o => o.TotalPrice)
            .HasColumnName("total_price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.OrderedAtUtc)
            .HasColumnName("ordered_at_utc")
            .IsRequired();

        builder.Property(o => o.LastSyncedAtUtc)
            .HasColumnName("last_synced_at_utc")
            .IsRequired();

        builder.HasIndex(o => o.ProductId);
    }
}
