using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleSales.Application.Catalogs.CreateCatalog;

public sealed record CreateCatalogCommand(
    string Name,
    string? Description) : ICommand<Guid>;
