using System.Reflection;

namespace ModularTemplate.Modules.Product.IntegrationEvents;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
