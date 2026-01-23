# Presentation Layer

The **Presentation Layer** handles the interface between the outside world and the application. It translates external requests into application commands/queries and formats responses.

## What Belongs Here

- **API Endpoints** - REST controllers, Minimal API handlers
- **Request/Response Models** - API-specific DTOs
- **Input Validation** - Request validation and sanitization
- **Error Handling** - Global exception handling, problem details
- **Authentication/Authorization** - Security concerns at the boundary

## Key Principle

> "The Interface Adapters layer is a set of adapters that convert data from the format most convenient for the use cases and entities, to the format most convenient for some external agency such as the Web."
> — Robert C. Martin, *Clean Architecture*

## Dependency Rule

Presentation depends on Application and Domain layers. It invokes use cases through the Application layer and never bypasses it to access the Domain directly for operations.

## References

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Robert C. Martin
- [ASP.NET Core Web API Best Practices](https://learn.microsoft.com/en-us/aspnet/core/web-api/) - Microsoft
- [REST API Guidelines](https://github.com/microsoft/api-guidelines) - Microsoft REST API Guidelines
