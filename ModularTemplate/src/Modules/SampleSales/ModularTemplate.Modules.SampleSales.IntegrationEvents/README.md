# Sales Integration Events

This project contains the public contracts (events) that the Sales module publishes for other modules to consume. This is the **only** project from the Sales module that other modules are allowed to reference.

## Event Flow

```
Sales Module (Publisher)
───────────────────────────────────────────────────────────────────
Domain Layer          Application Layer              Infrastructure
───────────────────────────────────────────────────────────────────
Product.Create()
  │
  ▼
ProductCreatedDomainEvent ──▶ ProductCreatedDomainEventHandler
                                       │
                                       ▼
                             IEventBus.PublishAsync(
                                ProductCreatedIntegrationEvent)
                                       │
                                       ▼
                                       Outbox ──▶ Message Bus
```

```
Orders Module (Consumer)
───────────────────────────────────────────────────────────────────
Infrastructure                  Presentation Layer (ACL)
───────────────────────────────────────────────────────────────────
Message Bus
  │
  ▼
IntegrationEventConsumer
  │
  ▼
inbox_messages table
  │
  ▼
ProcessInboxJob ──────────────▶ ProductCreatedIntegrationEventHandler
                                  │
                                  │ (updates local cache)
                                  ▼
                                IProductCacheWriter.UpsertAsync()
```

## Cross-Module Communication Pattern

The Sales module is the **source of truth** for product data. When products change:

1. Sales module publishes integration events
2. Orders module subscribes and updates its `ProductCache`
3. Orders module can query products without direct dependency on Sales
4. Eventual consistency is maintained via the inbox pattern

This enables:
- **Loose coupling** - No direct module dependencies
- **Data sovereignty** - Each module owns its data
- **Resilience** - Inbox pattern handles transient failures

## Dependency Rule

This project has **NO dependencies** on other layers or modules. It is a pure contract package containing only:
- Integration event record definitions
- Shared primitive types if needed

```
┌─────────────────────────────────────────────────────────────────┐
│                         Other Modules                           │
│   (Can ONLY reference this IntegrationEvents project)           │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│            Sales.IntegrationEvents (this project)               │
│   • ProductCreatedIntegrationEvent                              │
│   • ProductUpdatedIntegrationEvent                              │
└─────────────────────────────────────────────────────────────────┘
                              ▲
                              │ (publishes)
┌─────────────────────────────────────────────────────────────────┐
│                     Sales Module Internal                        │
│   • Domain (domain events trigger integration events)           │
│   • Application (handlers publish integration events)           │
│   • Infrastructure (sends to message bus)                       │
└─────────────────────────────────────────────────────────────────┘
```

## References

- [Integration Events](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications) - Microsoft
- [Event-Driven Architecture](https://martinfowler.com/articles/201701-event-driven.html) - Martin Fowler
