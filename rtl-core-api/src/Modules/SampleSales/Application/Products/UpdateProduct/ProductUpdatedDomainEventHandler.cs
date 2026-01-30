using Rtl.Core.Application.EventBus;
using Rtl.Core.Application.Messaging;
using Rtl.Core.Domain;
using Rtl.Module.SampleSales.Domain.Products;
using Rtl.Module.SampleSales.Domain.Products.Events;
using Rtl.Module.SampleSales.IntegrationEvents;

namespace Rtl.Module.SampleSales.Application.Products.UpdateProduct;

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
