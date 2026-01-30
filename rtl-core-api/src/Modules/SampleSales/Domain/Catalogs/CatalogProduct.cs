using Rtl.Core.Domain.Entities;
using Rtl.Core.Domain.ValueObjects;

namespace Rtl.Module.SampleSales.Domain.Catalogs;

/// <summary>
/// Entity representing a product within a Catalog aggregate.
/// Not an aggregate root - can only be accessed through the Catalog aggregate.
/// </summary>
public sealed class CatalogProduct : Entity
{
    private CatalogProduct()
    {
    }

    public Guid Id { get; private set; }

    public Guid CatalogId { get; private set; }

    public Guid ProductId { get; private set; }

    public Money? CustomPrice { get; private set; }

    public DateTime AddedAtUtc { get; private set; }

    internal static CatalogProduct Create(Guid catalogId, Guid productId, Money? customPrice = null)
    {
        return new CatalogProduct
        {
            Id = Guid.NewGuid(),
            CatalogId = catalogId,
            ProductId = productId,
            CustomPrice = customPrice,
            AddedAtUtc = DateTime.UtcNow
        };
    }

    internal void UpdateCustomPrice(Money? customPrice)
    {
        CustomPrice = customPrice;
    }
}
