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

The dedicated API projects (Customer, Fees, Organization, Product, Sales, SampleOrders, SampleSales) exist to give you flexibility during local development:

- **Faster startup** - Load only the module you're working on
- **Isolated debugging** - Focus on one module without noise from others
- **Database flexibility** - Test different database configurations per module

These are **not deployed to production**. In production, only `ModularTemplate.Api` runs with all modules loaded.

## Database Configuration Options

During local development, you have flexibility in how you structure your databases:

### Option 1: Single Database, Multiple Schemas (Recommended)
All modules share one database but each module has its own schema (e.g., `sampleorders`, `customer`, `sales`). This is the default configuration and mirrors production.

```
Database: ModularTemplate
├── Schema: samplesales
├── Schema: sampleorders
├── Schema: organization
├── Schema: customer
├── Schema: sales
├── Schema: fees
└── Schema: products
```

### Option 2: Separate Databases Per Module
Each module gets its own database. Useful when you want complete isolation or are testing module extraction.

```
Database: ModularTemplate_SampleSales
Database: ModularTemplate_SampleOrders
Database: ModularTemplate_Organization
Database: ModularTemplate_Customer
Database: ModularTemplate_Sales
Database: ModularTemplate_Fees
Database: ModularTemplate_Products
```

Configure this in each module's `appsettings.Development.json` by changing the connection string.

## Quick Start

**Running the full monolith:**
```bash
dotnet run --project src/API/ModularTemplate.Api
```

**Running a single module:**
```bash
dotnet run --project src/API/ModularTemplate.Api.SampleOrders
```

## When to Use What

| Scenario | Use This |
|----------|----------|
| Normal development | `ModularTemplate.Api` |
| Working on one module only | `ModularTemplate.Api.{Module}` |
| Running integration tests | `ModularTemplate.Api` |
| Production deployment | `ModularTemplate.Api` |
