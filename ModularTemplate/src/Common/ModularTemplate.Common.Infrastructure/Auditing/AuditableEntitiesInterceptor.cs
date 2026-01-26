using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    IDateTimeProvider dateTimeProvider) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
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

        var entries = context.ChangeTracker
            .Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.CreatedByUserId = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                // Prevent modification of create audit fields
                entry.Property(e => e.CreatedAtUtc).IsModified = false;
                entry.Property(e => e.CreatedByUserId).IsModified = false;
            }

            // Always update modified fields on add or update
            entry.Entity.ModifiedAtUtc = utcNow;
            entry.Entity.ModifiedByUserId = userId;
        }
    }
}
