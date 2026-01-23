using MediatR;
using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Common.Application.Messaging;

/// <summary>
/// Handler for queries.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
