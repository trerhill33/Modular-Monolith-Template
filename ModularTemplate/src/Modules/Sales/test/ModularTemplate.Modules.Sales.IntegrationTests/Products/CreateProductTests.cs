using FluentAssertions;
using ModularTemplate.Common.Domain.Results;
using ModularTemplate.Common.IntegrationTests.Abstractions;
using ModularTemplate.Modules.Sales.Application.Products.CreateProduct;
using ModularTemplate.Modules.Sales.Application.Products.GetProduct;
using ModularTemplate.Modules.Sales.IntegrationTests.Abstractions;

namespace ModularTemplate.Modules.Sales.IntegrationTests.Products;

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
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnProduct_WhenProductExists()
    {
        // Arrange
        Guid productId = await Sender.CreateProductAsync();

        // Act
        Result<ProductResponse> result = await Sender.Send(new GetProductQuery(productId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(productId);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateProductCommand("", "Description", 100);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
