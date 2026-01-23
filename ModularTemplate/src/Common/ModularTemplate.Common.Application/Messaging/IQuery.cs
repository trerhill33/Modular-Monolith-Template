using MediatR;
using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Common.Application.Messaging;

/// <summary>
/// Represents a query that returns a value.
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
