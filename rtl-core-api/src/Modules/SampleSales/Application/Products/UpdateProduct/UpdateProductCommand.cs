using Rtl.Core.Application.Messaging;

namespace Rtl.Module.SampleSales.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive) : ICommand;
