using System.Reflection;

namespace ModularTemplate.Modules.Sample.Presentation;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
