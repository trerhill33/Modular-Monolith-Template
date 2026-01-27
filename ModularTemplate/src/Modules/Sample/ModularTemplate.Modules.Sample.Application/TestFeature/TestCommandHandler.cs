using ModularTemplate.Common.Application.Messaging;
using ModularTemplate.Common.Domain.Results;

namespace ModularTemplate.Modules.Sample.Application.TestFeature;

// Test file - should trigger CI check for missing TestCommandHandlerTests.cs
internal sealed class TestCommandHandler : ICommandHandler<TestCommand, Guid>
{
    public Task<Result<Guid>> Handle(TestCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult<Result<Guid>>(Guid.NewGuid());
    }
}
