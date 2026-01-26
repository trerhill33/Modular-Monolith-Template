using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Caching;
using ModularTemplate.Common.Domain.Caching;

namespace ModularTemplate.Common.Infrastructure.Caching;

/// <summary>
/// EF Core interceptor that prevents accidental persistence of cache projection entities.
/// Cache entities can only be saved when an explicit write scope is active.
/// </summary>
public sealed class CacheWriteGuardInterceptor(
    ICacheWriteScope cacheWriteScope,
    ILogger<CacheWriteGuardInterceptor> logger) : SaveChangesInterceptor
{
    private readonly ICacheWriteScope _cacheWriteScope = cacheWriteScope;
    private readonly ILogger<CacheWriteGuardInterceptor> _logger = logger;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _logger.LogDebug("[CacheWriteGuardInterceptor] SavingChanges (sync) triggered. IsWriteAllowed={IsWriteAllowed}",
            _cacheWriteScope.IsWriteAllowed);

        if (eventData.Context is not null)
        {
            GuardCacheWrites(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CacheWriteGuardInterceptor] SavingChangesAsync triggered. IsWriteAllowed={IsWriteAllowed}",
            _cacheWriteScope.IsWriteAllowed);

        if (eventData.Context is not null)
        {
            GuardCacheWrites(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void GuardCacheWrites(DbContext context)
    {
        if (_cacheWriteScope.IsWriteAllowed)
        {
            _logger.LogDebug("[CacheWriteGuardInterceptor] Write scope is allowed, skipping guard check");
            return;
        }

        var cacheEntities = context.ChangeTracker
            .Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Where(e => e.Entity is ICacheProjection)
            .Select(e => e.Entity.GetType().Name)
            .Distinct()
            .ToList();

        if (cacheEntities.Count > 0)
        {
            _logger.LogError("[CacheWriteGuardInterceptor] BLOCKED cache write attempt for entities: {CacheEntities}",
                string.Join(", ", cacheEntities));

            throw new InvalidOperationException(
                $"Attempted to modify cache projection entities [{string.Join(", ", cacheEntities)}] " +
                "outside of an authorized write scope. Cache entities can only be modified within " +
                "integration event handlers using ICacheWriteScope.AllowWrites().");
        }

        _logger.LogDebug("[CacheWriteGuardInterceptor] No cache entities in change tracker, proceeding");
    }
}
