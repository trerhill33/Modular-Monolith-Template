using ModularTemplate.Common.Application.Messaging;

namespace ModularTemplate.Modules.Sample.Application.TestFeature;

// Test file - should trigger CI check for missing tests
public sealed record TestCommand(string Name) : ICommand<Guid>;
