using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.SampleSales.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive) : ICommand;
