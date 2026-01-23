using Bogus;
using MediatR;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Modules.Sales.Application.Products.CreateProduct;

namespace ModularTemplate.Modules.Sales.IntegrationTests.Abstractions;

internal static class CommandHelpers
{
    internal static async Task<Guid> CreateProductAsync(
        this ISender sender,
        string? name = null,
        string? description = null,
        decimal? price = null)
    {
        var faker = new Faker();
        var command = new CreateProductCommand(
            name ?? faker.Commerce.ProductName(),
            description ?? faker.Lorem.Sentence(),
            price ?? faker.Random.Decimal(1, 1000));

        Result<Guid> result = await sender.Send(command);
        return result.Value;
    }
}
