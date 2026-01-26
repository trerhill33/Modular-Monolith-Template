using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Identity;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Domain.Entities;

namespace ModularTemplate.Common.Infrastructure.Auditing;

/// <summary>
/// EF Core interceptor that automatically populates audit fields
/// on entities implementing IAuditableEntity during SaveChanges.
/// </summary>
public sealed class AuditableEntitiesInterceptor(
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider,
    ILogger<AuditableEntitiesInterceptor> logger) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<AuditableEntitiesInterceptor> _logger = logger;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[AuditInterceptor] SavingChangesAsync triggered. Context is {ContextStatus}",
            eventData.Context is not null ? "present" : "NULL");

        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _logger.LogDebug("[AuditInterceptor] SavingChanges (sync) triggered. Context is {ContextStatus}",
            eventData.Context is not null ? "present" : "NULL");

        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        var utcNow = _dateTimeProvider.UtcNow;
        var userId = _currentUserService.UserId ?? Guid.Empty;

        _logger.LogDebug("[AuditInterceptor] UpdateAuditableEntities called. UtcNow={UtcNow}, UserId={UserId}, DateTimeProviderType={ProviderType}",
            utcNow, userId, _dateTimeProvider.GetType().FullName);

        var entries = context.ChangeTracker
            .Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        _logger.LogDebug("[AuditInterceptor] Found {EntryCount} auditable entities to process", entries.Count);

        foreach (var entry in entries)
        {
            _logger.LogDebug("[AuditInterceptor] Processing entity {EntityType} with State={State}, Id={EntityId}",
                entry.Entity.GetType().Name, entry.State, GetEntityId(entry.Entity));

            if (entry.State == EntityState.Added)
            {
                _logger.LogDebug("[AuditInterceptor] Setting CreatedAtUtc={UtcNow} and CreatedByUserId={UserId} for ADDED entity",
                    utcNow, userId);

                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.CreatedByUserId = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                // Prevent modification of create audit fields
                entry.Property(e => e.CreatedAtUtc).IsModified = false;
                entry.Property(e => e.CreatedByUserId).IsModified = false;

                _logger.LogDebug("[AuditInterceptor] MODIFIED entity - preserving CreatedAtUtc and CreatedByUserId");
            }

            // Always update modified fields on add or update
            entry.Entity.ModifiedAtUtc = utcNow;
            entry.Entity.ModifiedByUserId = userId;

            _logger.LogDebug("[AuditInterceptor] After update - CreatedAtUtc={CreatedAtUtc}, ModifiedAtUtc={ModifiedAtUtc}",
                entry.Entity.CreatedAtUtc, entry.Entity.ModifiedAtUtc);
        }
    }

    private static object? GetEntityId(AuditableEntity entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        return idProperty?.GetValue(entity);
    }
}
