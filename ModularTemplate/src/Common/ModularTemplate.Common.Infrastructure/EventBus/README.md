# Event Bus

The Event Bus provides asynchronous communication between modules using integration events.

## Architecture

![Event_Bus_Arch_IMG]("../../../../../../docs/architecture/Event_Bus_Arch_IMG.png)

## Components

| Component | Purpose |
|-----------|---------|
| `IEventBus` | Publish integration events |
| `IEventDispatcher` | Route events to handlers |
| `IIntegrationEventHandler<T>` | Handle incoming events |
| `InMemoryEventBus` | Development: synchronous dispatch |
| `EventBridgeEventBus` | Production: publish to AWS EventBridge |
| `SqsPollingJobBase` | Production: poll SQS for events |

## Usage

### Publishing Events

```csharp
// In a domain event handler or application service
await eventBus.PublishAsync(
    new OrderCreatedIntegrationEvent(...),
    cancellationToken);
```

### Handling Events

```csharp
// In the consuming module's Application layer
internal sealed class OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
    : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken ct)
    {
        ...
        return Task.CompletedTask;
    }
}
```

## Configuration

### Development

No configuration needed. `InMemoryEventBus` is automatically registered.

### Production

```json
{
  "AwsMessaging": {
    "EventBusName": "modular-template-events",
    "EventSource": "ModularTemplate",
    "SqsQueueUrl": "https://sqs.us-east-1.amazonaws.com/123456789/my-module-queue",
    "PollingIntervalSeconds": 5,
    "MaxMessages": 10,
    "VisibilityTimeoutSeconds": 30
  }
}
```

## Module Setup

Each module needs:

1. **Integration event handlers** in `Application/IntegrationEvents/`
2. **SQS polling job** in `Infrastructure/EventBus/`

```csharp
// Infrastructure/EventBus/ProcessSqsJob.cs
internal sealed class ProcessSqsJob(
    IAmazonSQS sqsClient,
    IEventDispatcher eventDispatcher,
    IOptions<AwsMessagingOptions> options,
    ILogger<ProcessSqsJob> logger)
    : SqsPollingJobBase(sqsClient, eventDispatcher, options, logger)
{
    protected override string ModuleName => "MyModule";
}
```

```csharp
// In module registration
services.AddIntegrationEventHandlers(Application.AssemblyReference.Assembly);
services.AddSqsPolling<ProcessSqsJob>(environment);
```

## Idempotency

Handlers **must be idempotent** because SQS provides at-least-once delivery.

| Pattern | Example |
|---------|---------|
| Upsert | `INSERT ... ON CONFLICT DO UPDATE` |
| Check-then-act | `if (!exists) { create(); }` |
| Idempotency key | Store processed event IDs |

## Flow

```
Publishing Module                    Consuming Module
─────────────────                    ────────────────
1. Save entity (DB)
2. Save to outbox (same tx)
3. ProcessOutboxJob runs
4. IEventBus.PublishAsync()
        │
        ▼
   [EventBridge]
        │
        ▼
   [SQS Queue] ◄─── 5. ProcessSqsJob polls
        │
        ▼
   EventDispatcher ──→ 6. Handler invoked
```

## Related Documentation

- [Outbox Pattern](../Outbox/README.md) - Reliable event publishing
- [Messaging Architecture](../../../../docs/architecture/MESSAGING.md) - Full architecture details
- [EventBridge + SQS Roadmap](../../../../docs/architecture/EVENTBRIDGE-SQS-ROADMAP.md) - Implementation details
