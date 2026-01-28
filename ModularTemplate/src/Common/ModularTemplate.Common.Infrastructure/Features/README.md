# Feature Flags

Configuration-based feature toggles using `IConfiguration`.

## Two Tiers

| Scope | Config File | Example Path |
|-------|-------------|--------------|
| Infrastructure | `appsettings.json` | `Features:Infrastructure:Outbox` |
| Module | `modules.{module}.json` | `Sales:Features:CatalogV2Pagination` |

## Infrastructure Features

Global flags in `appsettings.json`:

```json
{
  "Features": {
    "Infrastructure": {
      "Outbox": true,
      "Inbox": true,
      "BackgroundJobs": true,
      "Emails": true,
      "CdcEvents": true
    }
  }
}
```

| Flag | Controls |
|------|----------|
| `Outbox` | Domain event publishing via outbox |
| `Inbox` | Integration event consumption |
| `BackgroundJobs` | Quartz job execution |
| `Emails` | Email sending |
| `CdcEvents` | Change Data Capture events |

**Defined in**: `InfrastructureFeatures.cs`

## Module Features

Per-module flags in `modules.{module}.json`:

```json
{
  "Sales": {
    "Features": {
      "CatalogV2Pagination": true,
      "BulkCreateProducts": false
    }
  }
}
```

**Define in module**:
```csharp
public static class SalesFeatures
{
    public const string CatalogV2Pagination = "Sales:Features:CatalogV2Pagination";
    public const string BulkCreateProducts = "Sales:Features:BulkCreateProducts";
}
```

## Usage

### In Endpoints (returns 404 when disabled)
```csharp
group.MapGet("/v2/products", GetAllV2Async)
    .RequireFeature(SalesFeatures.CatalogV2Pagination);
```

### In Handlers
```csharp
public class CreateProductHandler(IFeatureFlagService features)
{
    public async Task<Result<Guid>> Handle(...)
    {
        if (!await features.IsEnabledAsync(SalesFeatures.BulkCreateProducts))
            return Result.Failure(FeatureErrors.FeatureDisabled("BulkCreateProducts"));

        // Feature logic
    }
}
```

### In Background Jobs
```csharp
if (!_featureFlags.IsEnabled(InfrastructureFeatures.Outbox))
    return; // Skip processing
```

## Implementation

`ConfigurationFeatureFlagService` reads from `IConfiguration`:
- Path notation: `Section:Subsection:Flag`
- Missing flags default to `false` (disabled)
- Boolean parsing: `true`/`false` strings

## Microservice Extraction

Module flags live in module-specific config files. When extracting a module to a microservice, its feature flags move with it.
