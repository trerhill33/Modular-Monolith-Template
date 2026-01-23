# Domain Layer

The **Domain Layer** is the innermost layer of Clean Architecture. It contains the enterprise business rules and is completely independent of external concerns like databases, frameworks, or UI.

## What Belongs Here

- **Entities** - Objects with identity that encapsulate business logic
- **Value Objects** - Immutable objects defined by their attributes
- **Domain Events** - Notifications that something meaningful happened in the domain
- **Aggregates** - Clusters of entities and value objects treated as a single unit
- **Repository Interfaces** - Abstractions for data persistence (implementations live in Infrastructure)

## Key Principle

> "The Domain Layer has no dependencies on other layers. It is the core of the application and contains the business logic that would exist regardless of the technology used."
> — Robert C. Martin, *Clean Architecture*

## References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Robert C. Martin
- [Domain-Driven Design Reference](https://www.domainlanguage.com/ddd/reference/) - Eric Evans
- [.NET Architecture Guides](https://learn.microsoft.com/en-us/dotnet/architecture/) - Microsoft
