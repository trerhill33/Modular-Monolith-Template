using Rtl.Core.Domain.Entities;
using Rtl.Core.Domain.Results;
using Rtl.Core.Domain.ValueObjects;
using Rtl.Module.SampleSales.Domain.Catalogs.Events;

namespace Rtl.Module.SampleSales.Domain.Catalogs;

public sealed class Catalog : SoftDeletableEntity, IAggregateRoot
{
    private const int MaxNameLength = 200;
    private readonly List<CatalogProduct> _products = [];

    private Catalog()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public IReadOnlyCollection<CatalogProduct> Products => _products.AsReadOnly();

    public static Result<Catalog> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Catalog>(CatalogErrors.NameEmpty);
        }

        if (name.Length > MaxNameLength)
        {
            return Result.Failure<Catalog>(CatalogErrors.NameTooLong);
        }

        var catalog = new Catalog
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim()
        };

        catalog.Raise(new CatalogCreatedDomainEvent(catalog.Id));

        return catalog;
    }

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(CatalogErrors.NameEmpty);
        }

        if (name.Length > MaxNameLength)
        {
            return Result.Failure(CatalogErrors.NameTooLong);
        }

        Name = name.Trim();
        Description = description?.Trim();

        Raise(new CatalogUpdatedDomainEvent(Id));

        return Result.Success();
    }

    public Result AddProduct(Guid productId, Money? customPrice = null)
    {
        if (_products.Any(p => p.ProductId == productId))
        {
            return Result.Failure(CatalogErrors.ProductAlreadyInCatalog);
        }

        var catalogProduct = CatalogProduct.Create(Id, productId, customPrice);
        _products.Add(catalogProduct);

        Raise(new CatalogProductAddedDomainEvent(Id, catalogProduct.Id, productId));

        return Result.Success();
    }

    public Result RemoveProduct(Guid productId)
    {
        var catalogProduct = _products.FirstOrDefault(p => p.ProductId == productId);
        if (catalogProduct is null)
        {
            return Result.Failure(CatalogErrors.ProductNotInCatalog);
        }

        _products.Remove(catalogProduct);

        Raise(new CatalogProductRemovedDomainEvent(Id, catalogProduct.Id, productId));

        return Result.Success();
    }
}
