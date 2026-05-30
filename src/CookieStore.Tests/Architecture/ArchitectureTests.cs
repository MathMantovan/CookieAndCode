using FluentAssertions;
using NetArchTest.Rules;

namespace CookieStore.Tests.Architecture;

/// <summary>
/// Architecture tests that enforce DDD layer-dependency rules using NetArchTest.
/// These tests verify that no layer reaches across its permitted boundary,
/// keeping the Domain free of infrastructure concerns and the Application free
/// of infrastructure concerns.
/// </summary>
public class ArchitectureTests
{
    private const string DomainNamespace = "CookieStore.Domain";
    private const string ApplicationNamespace = "CookieStore.Application";
    private const string InfrastructureNamespace = "CookieStore.Infrastructure";

    private static readonly System.Reflection.Assembly DomainAssembly =
        typeof(CookieStore.Domain.DomainException).Assembly;

    private static readonly System.Reflection.Assembly ApplicationAssembly =
        typeof(CookieStore.Application.Sabores.SaborAppService).Assembly;

    private static readonly System.Reflection.Assembly InfrastructureAssembly =
        typeof(CookieStore.Infrastructure.CookieDbContext).Assembly;

    // -----------------------------------------------------------------------
    // Rule 1 — Domain must not reference Infrastructure
    //
    // DDD: The Domain layer is the innermost ring; it must have no knowledge
    // of persistence technology.  Any Infrastructure reference here would
    // break the Dependency-Inversion Principle that keeps the Domain pure.
    // -----------------------------------------------------------------------
    [Fact]
    public void Domain_ShouldNot_HaveDependencyOn_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "the Domain layer must never depend on Infrastructure — " +
                     "only the Infrastructure layer may depend on the Domain");
    }

    // -----------------------------------------------------------------------
    // Rule 2 — Domain must not reference Application
    //
    // DDD: Application Services orchestrate Domain objects, not the other
    // way around.  A Domain reference to Application would create a cycle
    // and violate the Clean-Architecture dependency direction.
    // -----------------------------------------------------------------------
    [Fact]
    public void Domain_ShouldNot_HaveDependencyOn_Application()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "the Domain layer must never depend on the Application layer — " +
                     "domain logic must remain independent of use-case orchestration");
    }

    // -----------------------------------------------------------------------
    // Rule 3 — Application must not reference Infrastructure
    //
    // DDD: The Application layer depends on Repository *interfaces* declared
    // in the Domain; it must never reach directly into the Infrastructure
    // implementations (EF Core, DbContext, etc.).
    // -----------------------------------------------------------------------
    [Fact]
    public void Application_ShouldNot_HaveDependencyOn_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: "the Application layer must program against Domain repository interfaces, " +
                     "not against Infrastructure implementations — this preserves testability");
    }
}
