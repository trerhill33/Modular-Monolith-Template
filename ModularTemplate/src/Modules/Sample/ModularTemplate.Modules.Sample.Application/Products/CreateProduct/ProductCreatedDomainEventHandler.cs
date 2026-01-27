using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.Sample.Domain.Products;
using ModularTemplate.Modules.Sample.Domain.Products.Events;
using ModularTemplate.Modules.Sample.IntegrationEvents;

namespace ModularTemplate.Modules.Sample.Application.Products.CreateProduct;

internal sealed class ProductCreatedDomainEventHandler(
    IProductRepository productRepository,
    IEventBus eventBus,
    IDateTimeProvider dateTimeProvider) : DomainEventHandler<ProductCreatedDomainEvent>
{
    public override async Task Handle(
        ProductCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Product? product = await productRepository.GetByIdAsync(
            domainEvent.ProductId,
            cancellationToken);

        if (product is null)
        {
            return;
        }

        await eventBus.PublishAsync(
            new ProductCreatedIntegrationEvent(
                Guid.NewGuid(),
                dateTimeProvider.UtcNow,
                product.Id,
                product.Name,
                product.Description,
                product.Price),
            cancellationToken);
    }
}
