using Microsoft.Extensions.Options;
using Quartz;

namespace ModularTemplate.Common.Infrastructure.Inbox.Job;

public sealed class ConfigureProcessInboxJob<TJob>(IOptions<InboxOptions> inboxOptions)
    : IConfigureOptions<QuartzOptions>
    where TJob : IJob
{
    private readonly InboxOptions _inboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        var jobName = typeof(TJob).FullName!;

        options
            .AddJob<TJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_inboxOptions.IntervalInSeconds).RepeatForever()));
    }
}
