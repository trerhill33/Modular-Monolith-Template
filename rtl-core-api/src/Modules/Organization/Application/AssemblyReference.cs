using System.Reflection;

namespace Rtl.Module.Organization.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
