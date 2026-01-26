using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.UpdateCatalog;

public sealed record UpdateCatalogCommand(
    Guid CatalogId,
    string Name,
    string? Description) : ICommand;
