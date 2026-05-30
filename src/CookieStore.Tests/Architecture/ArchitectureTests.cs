using FluentAssertions;
using NetArchTest.Rules;

namespace CookieStore.Tests.Architecture;

/// <summary>
/// Architecture tests that enforce DDD layer-dependency rules using NetArchTest.
/// These tests verify that no layer reaches across its permitted boundary,
/// keeping the Domain free of infrastructure concerns and the Application free
/// of infrastructure and presentation concerns.
/// </summary>
public class ArchitectureTests
{
    // Namespace constants used by NetArchTest.Rules to identify each assembly.
    private const string DomainNamespace = "CookieStore.Domain";
    private const string ApplicationNamespace = "CookieStore.Application";
    private const string InfrastructureNamespace = "CookieStore.Infrastructure";
    private const string ApiNamespace = "CookieStore.API";

    // -----------------------------------------------------------------------
    // Anchor types — one well-known public type per assembly so that
    // typeof(T).Assembly resolves the correct loaded assembly at runtime.
    // -----------------------------------------------------------------------
    private static readonly System.Reflection.Assembly DomainAssembly =
        typeof(CookieStore.Domain.DomainException).Assembly;

    private static readonly System.Reflection.Assembly ApplicationAssembly =
        typeof(CookieStore.Application.Sabores.SaborAppService).Assembly;

    private static readonly System.Reflection.Assembly InfrastructureAssembly =
        typeof(CookieStore.Infrastructure.CookieDbContext).Assembly;

    private static readonly System.Reflection.Assembly ApiAssembly =
        typeof(CookieStore.API.AssemblyAnchor).Assembly;

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
        // Arrange + Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
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
        // Arrange + Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        // Assert
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
        // Arrange + Act
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "the Application layer must program against Domain repository interfaces, " +
                     "not against Infrastructure implementations — this preserves testability");
    }

    // -----------------------------------------------------------------------
    // Rule 4 — Application must not reference API
    //
    // DDD: The Application layer must be presentation-agnostic.  An outward
    // reference to the API (controllers, routing, HTTP concerns) would couple
    // business orchestration to a delivery mechanism.
    // -----------------------------------------------------------------------
    [Fact]
    public void Application_ShouldNot_HaveDependencyOn_API()
    {
        // Arrange + Act
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "the Application layer must not know about the API layer — " +
                     "use cases must be independent of the delivery mechanism");
    }

    // -----------------------------------------------------------------------
    // Rule 5 — API must not reference Domain directly
    //
    // DDD: Controllers must talk to Application Services only.  A direct
    // reference from the API to Domain types (Aggregates, VOs, Repositories)
    // bypasses the Application layer and leaks domain objects into the
    // presentation tier, violating the Facade pattern of App Services.
    // -----------------------------------------------------------------------
    [Fact]
    public void API_ShouldNot_HaveDependencyOn_Domain()
    {
        // Arrange + Act
        var result = Types.InAssembly(ApiAssembly)
            .ShouldNot()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            because: "the API layer must only depend on Application Services and DTOs — " +
                     "Domain types must never be exposed directly through controllers");
    }
}
