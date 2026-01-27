# Organization Integration Events

This project contains integration events published by the Organization module for cross-module communication.

## Usage

Other modules can subscribe to these events by implementing `IIntegrationEventHandler<TEvent>` in their Presentation layer.

## Events

Add integration events here as the module evolves. Example:

```csharp
public sealed record OrganizationCreatedIntegrationEvent(
    Guid Id,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    string Name) : IntegrationEvent(Id, OccurredOnUtc);
```
