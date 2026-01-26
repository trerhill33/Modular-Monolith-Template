using Microsoft.EntityFrameworkCore;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Infrastructure.Auditing;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        // Outbox pattern configurations
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());

        // Inbox pattern configurations
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        // Audit trail configuration
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
    }
}
