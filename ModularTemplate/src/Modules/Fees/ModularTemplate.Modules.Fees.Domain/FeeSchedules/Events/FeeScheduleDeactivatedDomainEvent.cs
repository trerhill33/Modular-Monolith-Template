using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Fees.Domain.FeeSchedules.Events;

public sealed record FeeScheduleDeactivatedDomainEvent(Guid FeeScheduleId) : DomainEvent;
