using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleSales.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price) : ICommand<Guid>;
