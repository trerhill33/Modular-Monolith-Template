using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sample.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price) : ICommand<Guid>;
