using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sales.Application.Catalogs.CreateCatalog;

public sealed record CreateCatalogCommand(
    string Name,
    string? Description) : ICommand<Guid>;
