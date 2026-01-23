using ModularTemplate.Common.Domain;

namespace ModularTemplate.Common.Infrastructure.Clock;

/// <summary>
/// Default implementation of IDateTimeProvider.
/// </summary>
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
