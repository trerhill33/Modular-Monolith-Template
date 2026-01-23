# ModularTemplate.Api - Modular Monolith Template

A production-ready .NET 10 modular monolith template implementing Domain-Driven Design (DDD), Clean Architecture, and CQRS patterns.

## Features

- **Modular Architecture**: Loosely coupled modules with clear boundaries
- **Domain-Driven Design**: Entity base classes, domain events, aggregate roots
- **CQRS Pattern**: Separate command and query paths with MediatR
- **Result Pattern**: Railway-oriented programming for error handling
- **Outbox Pattern**: Reliable domain event publishing
- **Modern C#**: File-scoped namespaces, primary constructors, collection expressions

## Project Structure

```
ModularTemplate.Api/
├── src/
│   ├── API/
│   │   └── ModularTemplate.Api/          # Web API host
│   ├── Common/
│   │   ├── ModularTemplate.Common.Domain/         # Domain primitives
│   │   ├── ModularTemplate.Common.Application/    # CQRS, behaviors
│   │   ├── ModularTemplate.Common.Infrastructure/ # Database, caching, auth
│   │   └── ModularTemplate.Common.Presentation/   # Endpoints, API results
│   └── Modules/
│       └── Sample/                            # Example module
│           ├── ModularTemplate.Modules.Sample.Domain/
│           ├── ModularTemplate.Modules.Sample.Application/
│           ├── ModularTemplate.Modules.Sample.Infrastructure/
│           └── ModularTemplate.Modules.Sample.Presentation/
└── test/
    └── ModularTemplate.ArchitectureTests/        # Architecture validation
```

## Layer Dependencies

```
Domain          → (no dependencies)
Application     → Domain
Infrastructure  → Application, Domain
Presentation    → Application, Domain
API             → Infrastructure, Presentation
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Redis (optional, falls back to in-memory cache)

### Configuration

Update `appsettings.json` with your connection strings:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=retailcore;Username=postgres;Password=postgres",
    "Cache": "localhost:6379"
  }
}
```

### Running the Application

```bash
cd src/API/ModularTemplate.Api
dotnet run
```

The API will be available at `https://localhost:5001` with Swagger UI.

### Running Tests

```bash
dotnet test
```

## Creating a New Module

1. Create the module folder structure:
```
src/Modules/YourModule/
├── ModularTemplate.Modules.YourModule.Domain/
├── ModularTemplate.Modules.YourModule.Application/
├── ModularTemplate.Modules.YourModule.Infrastructure/
└── ModularTemplate.Modules.YourModule.Presentation/
```

2. Add project references following the layer dependency rules

3. Create your domain entities:
```csharp
public sealed class YourEntity : Entity
{
    public Guid Id { get; private set; }

    public static YourEntity Create(...)
    {
        var entity = new YourEntity { ... };
        entity.Raise(new YourEntityCreatedDomainEvent(entity.Id));
        return entity;
    }
}
```

4. Create commands and queries:
```csharp
public sealed record CreateYourEntityCommand(string Name) : ICommand<Guid>;

public sealed record GetYourEntityQuery(Guid Id) : IQuery<YourEntityResponse>;
```

5. Create handlers:
```csharp
internal sealed class CreateYourEntityCommandHandler(
    IYourEntityRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateYourEntityCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateYourEntityCommand request,
        CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

6. Create endpoints:
```csharp
internal sealed class CreateYourEntityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("your-entities", async (Request request, ISender sender) =>
        {
            var command = new CreateYourEntityCommand(request.Name);
            Result<Guid> result = await sender.Send(command);
            return result.Match(
                id => Results.Created($"/your-entities/{id}", new { id }),
                ApiResults.Problem);
        });
    }
}
```

7. Register the module in `Program.cs`:
```csharp
builder.Services.AddYourModule(builder.Configuration);
```

## Patterns Used

### Result Pattern

```csharp
public Result<Order> ProcessOrder(OrderRequest request)
{
    if (string.IsNullOrEmpty(request.CustomerId))
        return Result.Failure<Order>(Error.Problem("Order.InvalidCustomer", "Customer ID required"));

    var order = Order.Create(request);
    return Result.Success(order);
}
```

### Domain Events

```csharp
public sealed class OrderCreatedDomainEvent(Guid orderId) : DomainEvent;

// In entity
protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
```

### CQRS Commands and Queries

```csharp
// Command
public sealed record CreateOrderCommand(string CustomerId, List<OrderItem> Items) : ICommand<Guid>;

// Query
public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;
```

### Pipeline Behaviors

- **ValidationPipelineBehavior**: Validates commands using FluentValidation
- **RequestLoggingPipelineBehavior**: Logs request processing
- **ExceptionHandlingPipelineBehavior**: Handles unhandled exceptions

## Technology Stack

- .NET 10
- Entity Framework Core 9 (PostgreSQL)
- Dapper (read queries)
- MediatR
- FluentValidation
- MassTransit
- Quartz.NET
- Redis / StackExchange.Redis
- OpenTelemetry
- Serilog
- Swagger / OpenAPI

## License

This template is provided for internal use.
