using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Identity;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Domain.Entities;

namespace ModularTemplate.Common.Infrastructure.Auditing;

/// <summary>
/// EF Core interceptor that converts hard deletes to soft deletes for entities
/// implementing ISoftDeletable, and populates soft delete audit fields.
/// </summary>
/// <remarks>
/// When an entity implementing ISoftDeletable is marked for deletion,
/// this interceptor converts the delete operation to an update that sets
/// IsDeleted = true along with DeletedAtUtc and DeletedByUserId.
/// </remarks>
public sealed class SoftDeleteInterceptor(
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider,
    ILogger<SoftDeleteInterceptor> logger) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<SoftDeleteInterceptor> _logger = logger;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[SoftDeleteInterceptor] SavingChangesAsync triggered");

        if (eventData.Context is not null)
        {
            HandleSoftDeletes(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _logger.LogDebug("[SoftDeleteInterceptor] SavingChanges (sync) triggered");

        if (eventData.Context is not null)
        {
            HandleSoftDeletes(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    private void HandleSoftDeletes(DbContext context)
    {
        var utcNow = _dateTimeProvider.UtcNow;
        var userId = _currentUserService.UserId ?? Guid.Empty;

        var entries = context.ChangeTracker
            .Entries<SoftDeletableEntity>()
            .Where(e => e.State == EntityState.Deleted)
            .ToList();

        _logger.LogDebug("[SoftDeleteInterceptor] Found {EntryCount} entities marked for deletion", entries.Count);

        foreach (var entry in entries)
        {
            _logger.LogDebug("[SoftDeleteInterceptor] Converting hard delete to soft delete for {EntityType}",
                entry.Entity.GetType().Name);

            // Convert delete to update
            entry.State = EntityState.Modified;

            // Perform soft delete
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAtUtc = utcNow;
            entry.Entity.DeletedByUserId = userId;
        }
    }
}
