using System.Reflection;

namespace ModularTemplate.Modules.Product.Domain;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
