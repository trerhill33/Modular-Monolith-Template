namespace ModularTemplate.Modules.Sample.Application.Catalogs.GetCatalog;

public sealed record CatalogResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc,
    Guid CreatedByUserId,
    DateTime? ModifiedAtUtc,
    Guid? ModifiedByUserId);
