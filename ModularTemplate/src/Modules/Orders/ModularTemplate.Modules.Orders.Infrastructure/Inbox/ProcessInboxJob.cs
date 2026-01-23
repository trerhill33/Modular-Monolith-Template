using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModularTemplate.Common.Application.Data;
using ModularTemplate.Common.Domain;
using ModularTemplate.Common.Infrastructure.Inbox.Job;
using ModularTemplate.Modules.Orders.Infrastructure.Persistence;
using System.Reflection;

namespace ModularTemplate.Modules.Orders.Infrastructure.Inbox;

internal sealed class ProcessInboxJob(
    IDbConnectionFactory dbConnectionFactory,
    IServiceScopeFactory serviceScopeFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<InboxOptions> inboxOptions,
    ILogger<ProcessInboxJob> logger)
    : ProcessInboxJobBase(dbConnectionFactory, serviceScopeFactory, dateTimeProvider, inboxOptions, logger)
{
    protected override string ModuleName => "Orders";

    protected override string Schema => Schemas.Orders;

    protected override Assembly HandlersAssembly => Presentation.AssemblyReference.Assembly;
}
