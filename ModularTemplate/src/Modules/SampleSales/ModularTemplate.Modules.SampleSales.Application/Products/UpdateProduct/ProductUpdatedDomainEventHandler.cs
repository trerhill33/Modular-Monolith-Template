using ModularTemplate.Common.Application.EventBus;
using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain;
using ModularTemplate.Modules.SampleSales.Domain.Products;
using ModularTemplate.Modules.SampleSales.Domain.Products.Events;
using ModularTemplate.Modules.SampleSales.IntegrationEvents;

namespace ModularTemplate.Modules.SampleSales.Application.Products.UpdateProduct;

internal sealed class ProductUpdatedDomainEventHandler(
    IProductRepository productRepository,
    IEventBus eventBus,
    IDateTimeProvider dateTimeProvider) : DomainEventHandler<ProductUpdatedDomainEvent>
{
    public override async Task Handle(
        ProductUpdatedDomainEvent domainEvent,
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
            new ProductUpdatedIntegrationEvent(
                Guid.NewGuid(),
                dateTimeProvider.UtcNow,
                product.Id,
                product.Name,
                product.Description,
                product.Price.Amount,
                product.IsActive),
            cancellationToken);
    }
}
