# Orders Integration Events

This project contains the public contracts (events) that the Orders module publishes for other modules to consume. This is the **only** project from the Orders module that other modules are allowed to reference.


## Event Flow

```
Orders Module (Publisher)
───────────────────────────────────────────────────────────────────
Domain Layer          Application Layer              Infrastructure
───────────────────────────────────────────────────────────────────
Order.Place()
  │
  ▼
OrderPlacedDomainEvent ──▶ OrderPlacedDomainEventHandler
                                       │
                                       ▼
                             IEventBus.PublishAsync(
                                OrderPlacedIntegrationEvent)
                                       │
                                       ▼
                                       Outbox ──▶ Message Bus
```

```
Sales Module (Consumer)
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
ProcessInboxJob ──────────────▶ OrderPlacedIntegrationEventHandler
                                  │
                                  │ (updates local cache)
                                  ▼
                                IOrderCacheWriter.UpsertAsync()
```

## Cross-Module Communication Pattern

The Orders module is the **source of truth** for order data. When orders change:

1. Orders module publishes integration events
2. Sales module subscribes and updates its `OrderCache`
3. Sales module can query order information without direct dependency on Orders
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
│           Orders.IntegrationEvents (this project)               │
│   • OrderPlacedIntegrationEvent                                 │
│   • OrderStatusChangedIntegrationEvent                          │
└─────────────────────────────────────────────────────────────────┘
                              ▲
                              │ (publishes)
┌─────────────────────────────────────────────────────────────────┐
│                     Orders Module Internal                       │
│   • Domain (domain events trigger integration events)           │
│   • Application (handlers publish integration events)           │
│   • Infrastructure (sends to message bus)                       │
└─────────────────────────────────────────────────────────────────┘
```

## References

- [Integration Events](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/integration-event-based-microservice-communications) - Microsoft
- [Event-Driven Architecture](https://martinfowler.com/articles/201701-event-driven.html) - Martin Fowler
