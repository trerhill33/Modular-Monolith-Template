using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Modules.Sample.Domain.Catalogs;

public static class CatalogErrors
{
    public static Error NotFound(Guid catalogId) =>
        Error.NotFound("Catalogs.NotFound", $"The catalog with ID '{catalogId}' was not found.");

    public static readonly Error NameEmpty =
        Error.Validation("Catalogs.NameEmpty", "The catalog name cannot be empty.");
}
