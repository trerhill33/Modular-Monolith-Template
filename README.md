# ModularTemplate

A production-ready .NET 10 modular monolith template implementing Domain-Driven Design (DDD), Clean Architecture, and CQRS patterns.

## Quick Start

```bash
# Build
dotnet build ModularTemplate/ModularTemplate.Api.sln

# Test
dotnet test ModularTemplate/ModularTemplate.Api.sln

# Run
dotnet run --project ModularTemplate/src/API/ModularTemplate.Api
```

API available at `https://localhost:5001` with Swagger UI.

## Documentation

| Document | Description |
|----------|-------------|
| [Technical Docs](ModularTemplate/README.md) | Architecture, patterns, migrations, module creation |
| [AI Assistant Guide](CLAUDE.md) | Claude Code setup, skills, and agents |

## Repository Structure

```
├── ModularTemplate/
│   ├── src/
│   │   ├── API/                    # Web API host
│   │   ├── Common/                 # Shared kernel (Domain, Application, Infrastructure, Presentation)
│   │   └── Modules/                # Business modules (Orders, Sales, Customer, Organization, Sample)
│   └── test/                       # Architecture tests
├── .github/workflows/              # CI/CD pipelines
└── CLAUDE.md                       # AI assistant configuration
```

## Branching Strategy

Git Flow with `ses` as the integration branch. Feature branches merge to `ses`, which deploys to staging. Production releases merge from `ses` to `main`.

```
feature/PROJ-123 ──→ ses ──→ main
                       │        │
                       ▼        ▼
                   staging  production
```

### Environment Mapping

| Branch | Environments | Purpose |
|--------|--------------|---------|
| `ses` | staging | Integration testing, QA validation |
| `main` | production | Live releases |

### Branch Naming

| Type | Format | Example |
|------|--------|---------|
| Feature | `feature/{JIRA-TICKET}` | `feature/PROJ-123` |
| Bug fix | `bugfix/{JIRA-TICKET}` | `bugfix/PROJ-456` |
| Hotfix | `hotfix/{JIRA-TICKET}` | `hotfix/PROJ-789` |
| Release | `release/v{version}` | `release/v1.2.0` |

### Commit Messages

```
feat: add customer reassignment endpoint
fix: resolve null reference in order validation
refactor: consolidate DbConnectionFactory pattern
docs: update branching strategy
chore: update package dependencies
test: add integration tests for product creation
```

### Workflow

```bash
# Start feature work
git checkout ses && git pull
git checkout -b feature/PROJ-123

# Push and create PR to ses
git push -u origin feature/PROJ-123

# After merge to ses, cleanup
git checkout ses && git pull
git branch -d feature/PROJ-123

# Production release (from ses to main)
git checkout main && git pull
git merge ses
git push
```

### Branch Protection

| Branch | Rules |
|--------|-------|
| `ses` | Require PR approval, passing CI, no direct commits |
| `main` | Require PR approval, passing CI, merges from `ses` only |

### Required GitHub Secrets

For CI/CD deployment to work, configure these secrets in your repository:

| Secret | Description |
|--------|-------------|
| `AWS_ACCESS_KEY_ID` | AWS access key for ECR/ECS deployment |
| `AWS_SECRET_ACCESS_KEY` | AWS secret key for ECR/ECS deployment |
| `AWS_REGION` | AWS region (e.g., `us-east-1`) |

## CI/CD Pipeline

GitHub Actions automates build, test, and deployment:

### Build & Test (`build.yml`)
- Triggers on push/PR to `main` and `ses` branches
- Runs build and tests with code coverage collection
- Posts coverage report as PR comment (thresholds: 50% warning, 75% passing)
- On `main` push: builds Docker images per module via matrix strategy

### Deployment (`deploy-*.yml`)
- Per-module deployment workflows (Orders, Sales, Customer, Organization, Sample)
- Pushes images to Amazon ECR
- Deploys to AWS ECS with service stability checks
- Supports manual deployment via workflow dispatch

```
PR → Build/Test → Coverage Report
         │
         ▼ (on main)
    Docker Build (matrix: all modules)
         │
         ▼
    Push to ECR → Deploy to ECS
```

## Technology Stack

- .NET 10, Entity Framework Core 9, Dapper
- MediatR, FluentValidation, MassTransit
- Quartz.NET, Redis, PostgreSQL
- OpenTelemetry, Serilog, Swagger

## License

This template is provided for internal use.
