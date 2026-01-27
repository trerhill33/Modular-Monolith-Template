# Local Development Guide

## Overview

This guide covers different approaches to running ModularTemplate locally for development.

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL (local or Docker)
- Redis (optional, for caching)
- Docker (optional, for containerized development)

## Development Options

### Option 1: Run All Modules (Recommended for Most Development)

Run the unified API that includes all modules:

```bash
cd ModularTemplate
dotnet run --project src/API/ModularTemplate.Api
```

**URL:** `http://localhost:5000`

**When to use:**
- General feature development
- Testing cross-module functionality
- Exploring the full API

**Swagger UI:** `http://localhost:5000/swagger` (shows all modules in dropdown)

### Option 2: Run Specific Module Host

Run only the module you're working on:

```bash
# Run Orders module only
dotnet run --project src/API/ModularTemplate.Api.Orders

# Run Sales module only
dotnet run --project src/API/ModularTemplate.Api.Sales

# etc.
```

| Module | Command | URL |
|--------|---------|-----|
| Orders | `dotnet run --project src/API/ModularTemplate.Api.Orders` | `http://localhost:5001` |
| Sales | `dotnet run --project src/API/ModularTemplate.Api.Sales` | `http://localhost:5002` |
| Customer | `dotnet run --project src/API/ModularTemplate.Api.Customer` | `http://localhost:5003` |
| Inventory | `dotnet run --project src/API/ModularTemplate.Api.Inventory` | `http://localhost:5004` |
| Organization | `dotnet run --project src/API/ModularTemplate.Api.Organization` | `http://localhost:5005` |
| Sample | `dotnet run --project src/API/ModularTemplate.Api.Sample` | `http://localhost:5006` |

**When to use:**
- Focused development on single module
- Faster startup time
- Testing module isolation
- Simulating production deployment

### Option 3: Docker Compose (Full Stack)

Run everything in containers:

```bash
cd ModularTemplate
docker-compose up
```

See [Docker Compose Guide](./docker-compose.md) for details.

**When to use:**
- Testing Docker builds
- Simulating production environment
- Running integration tests
- Onboarding new developers

### Option 4: IDE Multiple Startup Projects

Configure your IDE to run multiple module hosts simultaneously.

#### Visual Studio

1. Right-click solution → **Set Startup Projects**
2. Select **Multiple startup projects**
3. Set Action to **Start** for:
   - `ModularTemplate.Api.Orders`
   - `ModularTemplate.Api.Sales`
   - etc.
4. Press F5 to start all

#### JetBrains Rider

1. **Run** → **Edit Configurations**
2. Click **+** → **Compound**
3. Add each module's run configuration
4. Run the compound configuration

## Database Setup

### Local PostgreSQL

Install PostgreSQL and create the development database:

```bash
# Connect to PostgreSQL
psql -U postgres

# Create databases for each module
CREATE DATABASE orders_dev;
CREATE DATABASE sales_dev;
CREATE DATABASE customer_dev;
CREATE DATABASE inventory_dev;
CREATE DATABASE organization_dev;
CREATE DATABASE sample_dev;

# Or create single database for all-in-one mode
CREATE DATABASE modulartemplate_dev;
```

### Docker PostgreSQL

```bash
docker run -d \
  --name postgres-dev \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16
```

### Connection Strings

**All-in-one mode (`ModularTemplate.Api`):**
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=modulartemplate_dev;Username=postgres;Password=postgres"
  }
}
```

**Per-module mode:**
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=orders_dev;Username=postgres;Password=postgres"
  }
}
```

## Running Migrations

### Development Mode (Automatic)

In Development environment, migrations run automatically on startup:

```csharp
// From MigrationExtensions.cs
if (environment.IsDevelopment())
{
    ApplyMigration<OrdersDbContext>(scope);
    // etc.
}
```

### Manual Migration

```bash
# Generate migration
dotnet ef migrations add InitialCreate \
  --project src/Modules/Orders/ModularTemplate.Modules.Orders.Infrastructure \
  --startup-project src/API/ModularTemplate.Api \
  --context OrdersDbContext

# Apply migration
dotnet ef database update \
  --project src/Modules/Orders/ModularTemplate.Modules.Orders.Infrastructure \
  --startup-project src/API/ModularTemplate.Api \
  --context OrdersDbContext
```

## Environment Configuration

### appsettings.Development.json

Override settings for local development:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=modulartemplate_dev;Username=postgres;Password=postgres",
    "Cache": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### User Secrets

For sensitive local settings:

```bash
# Initialize user secrets
dotnet user-secrets init --project src/API/ModularTemplate.Api

# Set secrets
dotnet user-secrets set "ConnectionStrings:Database" "Host=localhost;..." --project src/API/ModularTemplate.Api
```

## Debugging

### Visual Studio

1. Set breakpoints in code
2. Press F5 to start with debugger
3. Use Debug → Attach to Process for running containers

### VS Code

Add launch configuration in `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch All Modules",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/API/ModularTemplate.Api/bin/Debug/net9.0/ModularTemplate.Api.dll",
      "cwd": "${workspaceFolder}/src/API/ModularTemplate.Api"
    },
    {
      "name": "Launch Orders Module",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/API/ModularTemplate.Api.Orders/bin/Debug/net9.0/ModularTemplate.Api.Orders.dll",
      "cwd": "${workspaceFolder}/src/API/ModularTemplate.Api.Orders"
    }
  ]
}
```

### JetBrains Rider

1. Click gutter icon next to `Main` method
2. Select **Debug**
3. Or use Run → Debug configuration

## Hot Reload

Enable hot reload for faster development:

```bash
dotnet watch run --project src/API/ModularTemplate.Api
```

Changes to `.cs` files will automatically trigger rebuild.

## Testing Locally

### Run All Tests

```bash
dotnet test ModularTemplate.Api.sln
```

### Run Module Tests

```bash
# Unit tests
dotnet test src/Modules/Orders/test/ModularTemplate.Modules.Orders.Domain.Tests

# Integration tests
dotnet test src/Modules/Orders/test/ModularTemplate.Modules.Orders.IntegrationTests
```

### Watch Mode

```bash
dotnet watch test --project src/Modules/Orders/test/ModularTemplate.Modules.Orders.Domain.Tests
```

## Common Issues

### Port Already in Use

```
System.IO.IOException: Failed to bind to address http://localhost:5000
```

**Solution:** Kill the process using the port or use a different port:

```bash
# Find process
netstat -ano | findstr :5000

# Kill process
taskkill /PID <pid> /F
```

### Database Connection Failed

```
Npgsql.NpgsqlException: Failed to connect to localhost:5432
```

**Solution:** Ensure PostgreSQL is running:

```bash
# Docker
docker start postgres-dev

# Windows service
net start postgresql-x64-16
```

### Migration Pending

```
Microsoft.EntityFrameworkCore.Database.MigrationsException: The migration '...' has already been applied
```

**Solution:** Update database or remove conflicting migrations:

```bash
dotnet ef database update --context OrdersDbContext
```

## Performance Tips

1. **Use specific module host** when working on single module (faster startup)
2. **Disable migrations in development** if not changing schema
3. **Use in-memory database** for unit tests
4. **Enable response caching** for read-heavy development

## Related Documentation

- [Docker Compose Guide](./docker-compose.md)
- [Module Deployment Architecture](../architecture/module-deployment.md)
