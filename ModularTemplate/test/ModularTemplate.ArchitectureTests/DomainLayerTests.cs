using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace ModularTemplate.ArchitectureTests;

/// <summary>
/// Architecture tests for the Domain layer.
/// Domain is the innermost layer and should have no dependencies on outer layers.
/// Module domains may ONLY depend on Common.Domain.
/// </summary>
public sealed class DomainLayerTests : BaseTest
{
    #region Common.Domain Tests

    [Fact]
    public void CommonDomain_ShouldNotDependOn_Application()
    {
        if (CommonDomainAssembly is null)
        {
            Assert.Fail("Common.Domain assembly not found. Ensure naming convention is followed.");
            return;
        }

        var result = Types.InAssembly(CommonDomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(GetCommonNamespace("Application"))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Common.Domain should not depend on any Application layer");
    }

    [Fact]
    public void CommonDomain_ShouldNotDependOn_Infrastructure()
    {
        if (CommonDomainAssembly is null)
        {
            Assert.Fail("Common.Domain assembly not found. Ensure naming convention is followed.");
            return;
        }

        var result = Types.InAssembly(CommonDomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(GetCommonNamespace("Infrastructure"))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Common.Domain should not depend on any Infrastructure layer");
    }

    [Fact]
    public void CommonDomain_ShouldNotDependOn_Presentation()
    {
        if (CommonDomainAssembly is null)
        {
            Assert.Fail("Common.Domain assembly not found. Ensure naming convention is followed.");
            return;
        }

        var result = Types.InAssembly(CommonDomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(GetCommonNamespace("Presentation"))
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Common.Domain should not depend on any Presentation layer");
    }

    [Fact]
    public void CommonDomain_ShouldNotDependOn_AnyModule()
    {
        if (CommonDomainAssembly is null)
        {
            Assert.Fail("Common.Domain assembly not found. Ensure naming convention is followed.");
            return;
        }

        var moduleNamespaces = GetAllModuleNamespaces();
        if (moduleNamespaces.Length == 0)
        {
            return; // No modules to check against
        }

        var result = Types.InAssembly(CommonDomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(moduleNamespaces)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Common.Domain should not depend on any module-specific code");
    }

    #endregion

    #region Module Domain Tests (Auto-discovered)

    [Fact]
    public void AllModuleDomains_ShouldNotDependOn_Application()
    {
        var domains = GetModuleAssemblies("Domain");

        foreach (var (moduleName, assembly) in domains)
        {
            var forbiddenNamespaces = new[]
            {
                GetCommonNamespace("Application"),
                GetModuleNamespace(moduleName, "Application")
            };

            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenNamespaces)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"{moduleName}.Domain should not depend on any Application layer. " +
                $"Found dependencies in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void AllModuleDomains_ShouldNotDependOn_Infrastructure()
    {
        var domains = GetModuleAssemblies("Domain");

        foreach (var (moduleName, assembly) in domains)
        {
            var forbiddenNamespaces = new[]
            {
                GetCommonNamespace("Infrastructure"),
                GetModuleNamespace(moduleName, "Infrastructure")
            };

            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenNamespaces)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"{moduleName}.Domain should not depend on any Infrastructure layer. " +
                $"Found dependencies in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void AllModuleDomains_ShouldNotDependOn_Presentation()
    {
        var domains = GetModuleAssemblies("Domain");

        foreach (var (moduleName, assembly) in domains)
        {
            var forbiddenNamespaces = new[]
            {
                GetCommonNamespace("Presentation"),
                GetModuleNamespace(moduleName, "Presentation")
            };

            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(forbiddenNamespaces)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"{moduleName}.Domain should not depend on any Presentation layer. " +
                $"Found dependencies in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    [Fact]
    public void AllModuleDomains_ShouldNotDependOn_OtherModules()
    {
        var domains = GetModuleAssemblies("Domain");

        foreach (var (moduleName, assembly) in domains)
        {
            var otherModuleNamespaces = GetOtherModuleNamespaces(moduleName);
            if (otherModuleNamespaces.Length == 0)
            {
                continue; // Only one module exists
            }

            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherModuleNamespaces)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"{moduleName}.Domain should not depend on other modules. " +
                $"Found dependencies in: {string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())}");
        }
    }

    /// <summary>
    /// Domain CAN depend on Common.Domain (shared kernel).
    /// This test documents this allowed dependency.
    /// </summary>
    [Fact]
    public void DomainLayer_AllowedDependencies_Documentation()
    {
        // Domain layer is allowed to depend on:
        // - Common.Domain (shared kernel with base entities, value objects, domain events)
        //
        // Domain layer should NOT depend on:
        // - Application layer
        // - Infrastructure layer
        // - Presentation layer
        // - Other modules

        Assert.True(true, "Domain layer dependency rules documented above");
    }

    #endregion
}
