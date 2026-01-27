using System.Reflection;

namespace ModularTemplate.Modules.Customer.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
