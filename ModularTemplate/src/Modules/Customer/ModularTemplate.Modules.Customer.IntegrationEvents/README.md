# Customer Integration Events

This project contains integration events published by the Customer module for cross-module communication.

## Usage

Other modules can subscribe to these events by implementing `IIntegrationEventHandler<TEvent>` in their Presentation layer.

## Events

Add integration events here as the module evolves. Example:

```csharp
public sealed record CustomerCreatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid CustomerId,
    string Name,
    string Email) : IntegrationEvent(Id, OccurredOnUtc);
```
