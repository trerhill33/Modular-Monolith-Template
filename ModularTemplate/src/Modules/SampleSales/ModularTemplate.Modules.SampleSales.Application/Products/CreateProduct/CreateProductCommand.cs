using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleSales.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price) : ICommand<Guid>;
