using Rtl.Core.Application.EventBus;
using Rtl.Core.Application.Messaging;
using Rtl.Core.Domain;
using Rtl.Module.SampleSales.Domain.Products;
using Rtl.Module.SampleSales.Domain.Products.Events;
using Rtl.Module.SampleSales.IntegrationEvents;

namespace Rtl.Module.SampleSales.Application.Products.CreateProduct;

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
                product.Price.Amount),
            cancellationToken);
    }
}
