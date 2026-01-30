using Rtl.Core.Domain.Results;
using Rtl.Core.IntegrationTests.Abstractions;
using Rtl.Module.SampleSales.Application.Products.CreateProduct;

namespace Rtl.Module.SampleSales.IntegrationTests.Products;

public class CreateProductTests : BaseIntegrationTest
{
    public CreateProductTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Should_CreateProduct_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand(
            Faker.Commerce.ProductName(),
            Faker.Lorem.Sentence(),
            Faker.Random.Decimal(1, 1000));

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateProductCommand("", "Description", 100);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsFailure);
    }
}
