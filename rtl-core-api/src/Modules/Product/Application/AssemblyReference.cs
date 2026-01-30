using System.Reflection;

namespace Rtl.Module.Product.Application;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
