using MediatR;
using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Common.Application.Messaging;

/// <summary>
/// Handler for commands that return no value.
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;

/// <summary>
/// Handler for commands that return a value.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
