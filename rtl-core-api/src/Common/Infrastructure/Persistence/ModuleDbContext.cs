using Microsoft.EntityFrameworkCore;
using Rtl.Core.Application.Persistence;
using Rtl.Core.Infrastructure.Auditing.Configurations;
using Rtl.Core.Infrastructure.Inbox.Persistence;
using Rtl.Core.Infrastructure.Outbox.Persistence;

namespace Rtl.Core.Infrastructure.Persistence;

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
