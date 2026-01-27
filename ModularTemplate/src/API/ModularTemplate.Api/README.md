# API Projects

## Overview

This folder contains multiple API host projects. Understanding when and why to use each one will help you navigate local development.

## Projects

| Project | Purpose |
|---------|---------|
| `ModularTemplate.Api` | **Main monolith API** - runs all modules together. Use this for production and most development. |
| `ModularTemplate.Api.{Module}` | **Dedicated module APIs** - runs a single module in isolation. Use these for local development only. |
| `ModularTemplate.Api.Shared` | Shared extensions and configuration used by all API hosts. |

## Why Dedicated Module APIs Exist

The dedicated API projects (Customer, Orders, Organization, Sales, Sample) exist to give you flexibility during local development:

- **Faster startup** - Load only the module you're working on
- **Isolated debugging** - Focus on one module without noise from others
- **Database flexibility** - Test different database configurations per module

These are **not deployed to production**. In production, only `ModularTemplate.Api` runs with all modules loaded.

## Database Configuration Options

During local development, you have flexibility in how you structure your databases:

### Option 1: Single Database, Multiple Schemas (Recommended)
All modules share one database but each module has its own schema (e.g., `orders`, `customers`, `sales`). This is the default configuration and mirrors production.

```
Database: ModularTemplate
├── Schema: orders
├── Schema: customers
├── Schema: sales
└── Schema: sample
```

### Option 2: Separate Databases Per Module
Each module gets its own database. Useful when you want complete isolation or are testing module extraction.

```
Database: ModularTemplate_Orders
Database: ModularTemplate_Customers
Database: ModularTemplate_Sales
Database: ModularTemplate_Sample
```

Configure this in each module's `appsettings.Development.json` by changing the connection string.

## Quick Start

**Running the full monolith:**
```bash
dotnet run --project src/API/ModularTemplate.Api
```

**Running a single module:**
```bash
dotnet run --project src/API/ModularTemplate.Api.Orders
```

## When to Use What

| Scenario | Use This |
|----------|----------|
| Normal development | `ModularTemplate.Api` |
| Working on one module only | `ModularTemplate.Api.{Module}` |
| Running integration tests | `ModularTemplate.Api` |
| Production deployment | `ModularTemplate.Api` |
