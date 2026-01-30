using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleSales.Application.Catalogs.UpdateCatalog;

public sealed record UpdateCatalogCommand(
    Guid CatalogId,
    string Name,
    string? Description) : ICommand;
