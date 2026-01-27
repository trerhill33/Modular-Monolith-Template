using ModularTemplate.Common.Domain.Entities;
using ModularTemplate.Modules.Sample.Domain.Catalogs.Events;

namespace ModularTemplate.Modules.Sample.Domain.Catalogs;

public sealed class Catalog : SoftDeletableEntity
{
    private Catalog()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public static Catalog Create(string name, string? description)
    {
        var catalog = new Catalog
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };

        catalog.Raise(new CatalogCreatedDomainEvent(catalog.Id));

        return catalog;
    }

    public static void Update(Catalog catalog, string name, string? description)
    {
        catalog.Name = name;
        catalog.Description = description;

        catalog.Raise(new CatalogUpdatedDomainEvent(catalog.Id));
    }
}
