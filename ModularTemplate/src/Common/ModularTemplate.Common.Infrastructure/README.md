# Infrastructure Layer

The **Infrastructure Layer** provides implementations for external concerns. It is the outermost layer and contains all the "details" that the inner layers are protected from.

## What Belongs Here

- **Repository Implementations** - Database access using EF Core, Dapper, etc.
- **External Service Integrations** - Email, file storage, third-party APIs
- **Message Bus / Event Publishing** - MassTransit, RabbitMQ integrations
- **Caching** - Redis, in-memory cache implementations
- **Background Jobs** - Quartz.NET job definitions
- **Outbox/Inbox Pattern** - Reliable messaging implementations

## Key Principle

> "Frameworks and Drivers. This layer is where all the details go. The Web is a detail. The database is a detail. We keep these things on the outside where they can do little harm."
> — Robert C. Martin, *Clean Architecture*

## Dependency Rule

Infrastructure depends on Domain and Application layers. It implements the interfaces defined in those layers, providing concrete implementations for abstractions.

## References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Robert C. Martin
- [Infrastructure Layer](https://www.domainlanguage.com/ddd/reference/) - Eric Evans, DDD Reference
- [.NET Microservices Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/) - Microsoft
