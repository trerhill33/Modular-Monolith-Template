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
| `DbConnectionFactory.cs` | Creates database connections for Dapper queries |
| `Repository.cs` | Generic repository base class |
| `AuditableEntityConfiguration.cs` | EF Core config for audit fields |
| `SoftDeletableEntityConfiguration.cs` | EF Core config for soft delete |

## EF Core Migration Quick Reference

Run from solution root (`SES-Pro/`). Replace `{Module}` with your module name.

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
