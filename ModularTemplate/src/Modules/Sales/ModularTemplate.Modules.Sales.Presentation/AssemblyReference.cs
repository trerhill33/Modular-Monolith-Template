using System.Reflection;

namespace ModularTemplate.Modules.Sales.Presentation;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
