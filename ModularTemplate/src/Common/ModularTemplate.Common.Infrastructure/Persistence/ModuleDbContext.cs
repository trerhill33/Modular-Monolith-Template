using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.Inbox.Data;
using ModularTemplate.Common.Infrastructure.Outbox.Data;

namespace ModularTemplate.Common.Infrastructure.Persistence;

/// <summary>
/// Base DbContext for all modules, providing common infrastructure configurations.
/// </summary>
public abstract class ModuleDbContext<TContext>(DbContextOptions<TContext> options)
    : DbContext(options), IUnitOfWork
    where TContext : DbContext
{
    protected abstract string Schema { get; }

    /// <summary>
    /// Optional logger for tracking persistence operations. Injected via derived contexts if needed.
    /// </summary>
    protected ILogger? Logger { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        // Outbox pattern configurations
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());

        // Inbox pattern configurations
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        //TODO: add audti trails
        //modelBuilder.ApplyAuditConfigurations();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var contextType = GetType().Name;

        Logger?.LogDebug("[{ContextType}] SaveChangesAsync STARTING. Schema={Schema}", contextType, Schema);

        // Log all tracked entities
        var trackedEntities = ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
            .Select(e => new { Type = e.Entity.GetType().Name, e.State })
            .ToList();

        Logger?.LogDebug("[{ContextType}] Tracked entities: {TrackedEntities}",
            contextType, string.Join(", ", trackedEntities.Select(e => $"{e.Type}({e.State})")));

        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            Logger?.LogInformation("[{ContextType}] SaveChangesAsync COMPLETED. RowsAffected={RowsAffected}",
                contextType, result);

            return result;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "[{ContextType}] SaveChangesAsync FAILED: {ErrorMessage}",
                contextType, ex.Message);
            throw;
        }
    }

    public override int SaveChanges()
    {
        var contextType = GetType().Name;

        Logger?.LogDebug("[{ContextType}] SaveChanges (sync) STARTING. Schema={Schema}", contextType, Schema);

        try
        {
            var result = base.SaveChanges();

            Logger?.LogInformation("[{ContextType}] SaveChanges (sync) COMPLETED. RowsAffected={RowsAffected}",
                contextType, result);

            return result;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "[{ContextType}] SaveChanges (sync) FAILED: {ErrorMessage}",
                contextType, ex.Message);
            throw;
        }
    }
}
