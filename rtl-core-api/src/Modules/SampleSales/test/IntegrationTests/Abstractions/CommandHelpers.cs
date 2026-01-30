using Bogus;
using MediatR;
using Rtl.Core.Domain.Results;
using Rtl.Module.SampleSales.Application.Products.CreateProduct;

namespace Rtl.Module.SampleSales.IntegrationTests.Abstractions;

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
