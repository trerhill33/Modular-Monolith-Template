# Per-Module ECS Deployment Architecture

## Overview

ModularTemplate supports deploying each module as a separate ECS task with full isolation. This architecture enables:

- **Independent scaling**: Each module scales based on its own traffic patterns
- **Isolated failures**: Issues in one module don't affect others
- **Independent deployments**: Deploy modules without coordinating with other teams
- **Future extraction**: Modules can be extracted to their own repositories seamlessly

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        AWS ECS Cluster                          │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │ Orders Task │  │ Sales Task  │  │Customer Task│  ...        │
│  │   (Fargate) │  │  (Fargate)  │  │  (Fargate)  │             │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘             │
│         │                │                │                     │
│  ┌──────▼──────┐  ┌──────▼──────┐  ┌──────▼──────┐             │
│  │  Orders DB  │  │  Sales DB   │  │ Customer DB │  ...        │
│  │  (RDS/PG)   │  │  (RDS/PG)   │  │  (RDS/PG)   │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
```

## Project Structure

```
ModularTemplate/
├── src/
│   ├── API/
│   │   ├── ModularTemplate.Api.Shared/         # Shared extensions library
│   │   ├── ModularTemplate.Api.Orders/       # Dedicated host for Orders
│   │   ├── ModularTemplate.Api.Sales/        # Dedicated host for Sales
│   │   ├── ModularTemplate.Api.Customer/     # Dedicated host for Customer
│   │   ├── ModularTemplate.Api.Inventory/    # Dedicated host for Inventory
│   │   ├── ModularTemplate.Api.Organization/ # Dedicated host for Organization
│   │   ├── ModularTemplate.Api.Sample/       # Dedicated host for Sample
│   │   └── ModularTemplate.Api/              # All modules (local dev)
│   ├── Common/                               # Shared kernel
│   └── Modules/                              # Module implementations
```

## Component Details

### Shared Host Library (`ModularTemplate.Api.Shared`)

A class library containing reusable extensions for all module hosts:

- `HostExtensions.cs` - `AddModuleHostDefaults()` and `UseModuleHostDefaults()`
- `OpenApiExtensions.cs` - Swagger/OpenAPI configuration
- `ApiVersioningExtensions.cs` - API versioning setup
- `HealthCheckExtensions.cs` - Health check configuration
- `ExceptionHandlingExtensions.cs` - Global error handling

Module hosts reference this library and can:
1. Use shared extensions as-is
2. Override specific extensions when needed
3. Add module-specific middleware

### Per-Module Hosts

Each module host (`ModularTemplate.Api.{Module}`) contains:

| File | Purpose |
|------|---------|
| `Program.cs` | Minimal startup for single module |
| `appsettings.json` | Module-specific configuration |
| `appsettings.Development.json` | Local development overrides |
| `Dockerfile` | Container build configuration |
| `launchSettings.json` | IDE launch configuration |

### Database Strategy

Each module uses its own PostgreSQL database:

| Module | Production Database | Local Dev Database |
|--------|--------------------|--------------------|
| Orders | `orders-db.rds.amazonaws.com/orders` | `localhost/orders_dev` |
| Sales | `sales-db.rds.amazonaws.com/sales` | `localhost/sales_dev` |
| Customer | `customer-db.rds.amazonaws.com/customer` | `localhost/customer_dev` |
| Inventory | `inventory-db.rds.amazonaws.com/inventory` | `localhost/inventory_dev` |
| Organization | `organization-db.rds.amazonaws.com/org` | `localhost/organization_dev` |
| Sample | `sample-db.rds.amazonaws.com/sample` | `localhost/sample_dev` |

## Communication Between Modules

Modules communicate via **integration events** using the outbox/inbox pattern:

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Orders    │────►│   AWS SQS   │────►│    Sales    │
│   Module    │     │   Queue     │     │   Module    │
└─────────────┘     └─────────────┘     └─────────────┘
      │                                        │
      ▼                                        ▼
   Outbox                                   Inbox
   (Orders DB)                            (Sales DB)
```

**Key Points:**
- Events are stored in the source module's outbox table
- A background job publishes events to SQS
- Consuming modules poll SQS and store in their inbox table
- Inbox pattern ensures idempotent event processing

## Port Assignments (Local Development)

| Module | Port | URL |
|--------|------|-----|
| Orders | 5001 | `http://localhost:5001` |
| Sales | 5002 | `http://localhost:5002` |
| Customer | 5003 | `http://localhost:5003` |
| Inventory | 5004 | `http://localhost:5004` |
| Organization | 5005 | `http://localhost:5005` |
| Sample | 5006 | `http://localhost:5006` |
| All Modules | 5000 | `http://localhost:5000` |

## Docker Images

Each module produces its own Docker image:

```bash
# Build Orders module image
docker build \
  -f ModularTemplate/src/API/ModularTemplate.Api.Orders/Dockerfile \
  -t modular-api-orders:latest \
  .
```

Images are pushed to ECR:
- `{account}.dkr.ecr.{region}.amazonaws.com/modular-api-orders`
- `{account}.dkr.ecr.{region}.amazonaws.com/modular-api-sales`
- etc.

## ECS Configuration

Each module has its own:
- **Task Definition**: CPU/memory, container configuration
- **Service**: Desired count, deployment configuration
- **Target Group**: Health checks, load balancer integration

See `ecs/` directory for task definition templates.

## CI/CD Pipeline

### Build Phase (Matrix Build)

All modules are built in parallel using GitHub Actions matrix strategy:

```yaml
jobs:
  build:
    strategy:
      matrix:
        module: [Orders, Sales, Customer, Inventory, Organization, Sample]
```

### Deploy Phase (Per-Module)

Each module has its own deploy workflow:
- `deploy-orders.yml`
- `deploy-sales.yml`
- etc.

Deployments can be:
- **Automatic**: Triggered after successful build
- **Manual**: Using `workflow_dispatch`

## Benefits of This Architecture

| Benefit | Description |
|---------|-------------|
| **Independent scaling** | Scale Orders separately from Sales |
| **Fault isolation** | Orders failure doesn't affect Sales |
| **Team ownership** | Each team owns their module end-to-end |
| **Independent releases** | Deploy Orders without touching Sales |
| **Future extraction** | Module host can be extracted to own repo |

## Trade-offs

| Trade-off | Mitigation |
|-----------|------------|
| More infrastructure to manage | Use IaC (Terraform/CDK) |
| Cross-module queries harder | Use integration events for data sync |
| Distributed transactions | Design for eventual consistency |
| More complex local dev | Use docker-compose or run all-in-one API |

## Related Documentation

- [Module Extraction Guide](./module-extraction.md) - How to extract a module to its own repository
- [Local Development](../development/local-development.md) - Running modules locally
- [Docker Compose](../development/docker-compose.md) - Full stack local setup
- [ECS Task Definitions](../deployment/ecs-task-definitions.md) - ECS configuration reference
