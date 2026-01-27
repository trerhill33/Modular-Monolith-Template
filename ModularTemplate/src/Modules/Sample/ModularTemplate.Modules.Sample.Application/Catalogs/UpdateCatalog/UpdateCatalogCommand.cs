using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sample.Application.Catalogs.UpdateCatalog;

public sealed record UpdateCatalogCommand(
    Guid CatalogId,
    string Name,
    string? Description) : ICommand;
