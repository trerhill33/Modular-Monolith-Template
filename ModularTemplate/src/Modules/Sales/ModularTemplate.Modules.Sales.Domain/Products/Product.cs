using ModularTemplate.Common.Domain.Entities;
using ModularTemplate.Modules.Sales.Domain.Products.Events;

namespace ModularTemplate.Modules.Sales.Domain.Products;

public sealed class Product : SoftDeletableEntity
{
    private Product()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; } = true;

    public static Product Create(string name, string? description, decimal price)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            IsActive = true
        };

        product.Raise(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public static void Update(Product product, string name, string? description, decimal price, bool isActive)
    {
        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.IsActive = isActive;

        product.Raise(new ProductUpdatedDomainEvent(product.Id));
    }
}
