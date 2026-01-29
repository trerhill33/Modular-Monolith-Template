# Data Infrastructure

## Why No DbContext Here?

Each **module owns its own DbContext and database schema**. This is by design:

- Modules are isolated bounded contexts with their own data
- Teams can evolve their schemas independently
- Modules can be extracted to microservices without shared database coupling

Look for DbContexts in each module's Infrastructure layer:
```
src/Modules/{ModuleName}/ModularTemplate.Modules.{ModuleName}.Infrastructure/Database/
```

## What's in This Folder?

This folder contains **shared data infrastructure** used by all modules:

| File | Purpose |
|------|---------|
| `DbConnectionFactory<TModule>.cs` | Creates module-specific database connections for Dapper queries |
| `Repository.cs` | Generic repository base class |
| `AuditableEntityConfiguration.cs` | EF Core config for audit fields |
| `SoftDeletableEntityConfiguration.cs` | EF Core config for soft delete |

## Database Connection Factory

### Why Generic?

We use `IDbConnectionFactory<TModule>` (generic) instead of a shared `IDbConnectionFactory`:

```csharp
// Each module has its own connection factory
IDbConnectionFactory<ISampleOrdersModule>   // SampleOrders module connections
IDbConnectionFactory<ISampleSalesModule>    // SampleSales module connections
IDbConnectionFactory<ICustomerModule>       // Customer module connections
```

**Benefits:**
- **Module Isolation**: Each module can have its own database
- **DI Resolution**: The container resolves the correct factory per module
- **Consistency**: All components (jobs, handlers) use the same pattern

### Registration

Register in each module's setup:

```csharp
services.AddModuleDataSource<ISampleOrdersModule>(databaseConnectionString);
```

### Usage in Handlers

```csharp
public class ProcessOutboxJob(
    IDbConnectionFactory<ISampleOrdersModule> dbConnectionFactory,
    // ...
) : ProcessOutboxJobBase<ISampleOrdersModule>(dbConnectionFactory, ...)
```

## Database Configuration Options

### Single Database (Default)
All modules share one database with separate schemas:
```
PostgreSQL: modulartemplate
├── Schema: samplesales
├── Schema: sampleorders
├── Schema: customer
└── Schema: sales
```

### Multiple Databases
Each module can have its own database via config:
```json
{
  "Modules": {
    "SampleOrders": {
      "ConnectionStrings": {
        "Database": "Host=localhost;Database=modulartemplate_sampleorders;..."
      }
    }
  }
}
```

## EF Core Migration Quick Reference

Run from solution root. Replace `{Module}` with your module name.

**Create Migration:**
```bash
dotnet ef migrations add <Name> \
  --project src/Modules/{Module}/ModularTemplate.Modules.{Module}.Infrastructure \
  --startup-project src/API/ModularTemplate.Api
```

**Apply Migration:**
```bash
dotnet ef database update \
  --project src/Modules/{Module}/ModularTemplate.Modules.{Module}.Infrastructure \
  --startup-project src/API/ModularTemplate.Api
```

**Remove Last Migration:**
```bash
dotnet ef migrations remove \
  --project src/Modules/{Module}/ModularTemplate.Modules.{Module}.Infrastructure \
  --startup-project src/API/ModularTemplate.Api
```

**Generate SQL Script (Production):**
```bash
dotnet ef migrations script \
  --project src/Modules/{Module}/ModularTemplate.Modules.{Module}.Infrastructure \
  --startup-project src/API/ModularTemplate.Api \
  --idempotent \
  --output migrations.sql
```

For detailed migration documentation, see each module's `Database/README.md`.
