# Template Readiness Roadmap

Items to address before this template is ready for team use.

---

## Priority 1: Security & Configuration

### CORS Policy
Remove `AllowAnyOrigin()` default and require explicit configuration.

**Current** (Program.cs):
```csharp
.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
})
```

**Change to**:
```csharp
.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        policy
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
})
```

Add to appsettings:
```json
{
  "Cors": {
    "AllowedOrigins": ["https://localhost:3000"]
  }
}
```

---

## Priority 2: Package Updates

### Core Framework Packages (stay on .NET 9)
These show .NET 10 as "latest" but we should stay on 9.x versions:

- Microsoft.EntityFrameworkCore: 9.0.0 (keep)
- Microsoft.AspNetCore.OpenApi: 9.0.0 (keep)
- Microsoft.AspNetCore.Authentication.JwtBearer: 9.0.0 (keep)
- Microsoft.Extensions.Caching.StackExchangeRedis: 9.0.0 (keep)
- Npgsql.EntityFrameworkCore.PostgreSQL: 9.0.1 (keep)
- EFCore.NamingConventions: 9.0.0 (keep)

### Packages to Update

**Infrastructure:**
- AWSSDK.EventBridge: 3.7.401.10 → 4.0.5.14
- AWSSDK.Extensions.NETCore.Setup: 3.7.301 → 4.0.3.21
- AWSSDK.SQS: 3.7.400.63 → 4.0.2.13
- Dapper: 2.1.35 → 2.1.66
- Newtonsoft.Json: 13.0.3 → 13.0.4
- Quartz.Extensions.Hosting: 3.13.1 → 3.15.1

**API:**
- Asp.Versioning.Mvc.ApiExplorer: 8.1.0 → 8.1.1
- AspNetCore.HealthChecks.NpgSql: 8.0.2 → 9.0.0
- AspNetCore.HealthChecks.Redis: 8.0.1 → 9.0.0
- AspNetCore.HealthChecks.UI.Client: 8.0.1 → 9.0.0
- Swashbuckle.AspNetCore: 7.2.0 → stay on 7.x (10.x is major breaking change)

**Application:**
- FluentValidation.DependencyInjectionExtensions: 11.11.0 → 12.1.1
- MediatR: 12.4.1 → 14.0.0 (review breaking changes first)

**Testing:**
- FluentAssertions: 6.12.2/8.0.1 → standardize on 8.8.0 or replace
- Bogus: 35.6.1 → 35.6.5
- Microsoft.NET.Test.Sdk: 17.11.1/17.12.0 → 18.0.1
- Respawn: 6.2.1 → 7.0.0
- xunit: 2.9.2 → 2.9.3
- xunit.runner.visualstudio: 2.8.2/3.0.2 → 3.1.5

---

## Priority 3: FluentAssertions Decision

FluentAssertions changed licensing in v8. Options:

1. **Keep FluentAssertions 8.x** - Accept new license terms
2. **Switch to Shouldly** - Open source, similar syntax
3. **Use built-in xUnit assertions** - No dependency, less fluent

**Recommendation**: Evaluate Shouldly as replacement. Similar syntax, fully open source.

```csharp
// FluentAssertions
result.Should().Be(expected);

// Shouldly
result.ShouldBe(expected);
```

---

## Priority 4: Pagination Enhancement

Current IReadRepository only supports limit:
```csharp
Task<IReadOnlyCollection<TEntity>> GetAllAsync(int? limit = 100, ...);
```

Add PagedResult support:

**Add to Common.Domain:**
```csharp
public record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
```

**Update IReadRepository:**
```csharp
public interface IReadRepository<TEntity, in TId>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(int? limit = 100, CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
```

---

## Priority 5: Documentation Cleanup

- Remove Inventory module references from README.md (migration commands)
- Update .NET version references (says .NET 10 in some places, should be .NET 9)
- Add ARCHITECTURE-FAQ.md to explain design decisions

---

## Priority 6: Transitive Dependency Hygiene

Even though no vulnerabilities were detected, pin these transitive dependencies to ensure consistent versions:

Add to Directory.Packages.props or relevant csproj files:
```xml
<PackageReference Include="Azure.Identity" Version="1.13.2" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
<PackageReference Include="System.Formats.Asn1" Version="9.0.0" />
```

---

## Execution Order

1. CORS configuration change (quick win, security fix)
2. Package updates (batch update, test thoroughly)
3. FluentAssertions decision (team discussion needed)
4. Pagination enhancement (new feature)
5. Documentation cleanup (ongoing)
6. Transitive dependencies (housekeeping)

---

## Notes

- MediatR 14.0.0 has breaking changes - review migration guide before updating
- AWSSDK 4.x is a major version bump - test AWS integrations thoroughly
- Swashbuckle 10.x has significant breaking changes - recommend staying on 7.x for now
- Keep all Microsoft.* packages aligned on same minor version (9.0.x for .NET 9)
