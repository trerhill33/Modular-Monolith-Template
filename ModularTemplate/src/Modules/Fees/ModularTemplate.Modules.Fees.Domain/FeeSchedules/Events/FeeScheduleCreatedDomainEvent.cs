using ModularTemplate.Common.Domain.Events;

namespace ModularTemplate.Modules.Fees.Domain.FeeSchedules.Events;

public sealed record FeeScheduleCreatedDomainEvent(Guid FeeScheduleId) : DomainEvent;
