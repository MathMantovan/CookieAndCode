using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.SharedKernel;

/// <summary>
/// Tests for the abstract Entity base class (DDD: Entity has identity-based equality,
/// not structural equality — two instances with the same Id are the same entity).
/// </summary>
public class EntityTests
{
    // -----------------------------------------------------------------------
    // Concrete stub to exercise the abstract Entity
    // -----------------------------------------------------------------------

    private sealed class StubEntity : Entity
    {
        public StubEntity(Guid id) : base(id) { }
    }

    private sealed class AnotherStubEntity : Entity
    {
        public string Label { get; }

        public AnotherStubEntity(Guid id, string label) : base(id)
        {
            Label = label;
        }

        public override string Describe() => $"AnotherStubEntity({Label})";
    }

    // -----------------------------------------------------------------------
    // Equality
    // -----------------------------------------------------------------------

    [Fact]
    public void Entity_WithSameId_Equals_ShouldBeTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new StubEntity(id);
        var entity2 = new StubEntity(id);

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_WithSameId_EqualityOperator_ShouldBeTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        Entity entity1 = new StubEntity(id);
        Entity entity2 = new StubEntity(id);

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_WithDifferentIds_Equals_ShouldBeFalse()
    {
        // Arrange
        var entity1 = new StubEntity(Guid.NewGuid());
        var entity2 = new StubEntity(Guid.NewGuid());

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void Entity_WithDifferentIds_InequalityOperator_ShouldBeTrue()
    {
        // Arrange
        Entity entity1 = new StubEntity(Guid.NewGuid());
        Entity entity2 = new StubEntity(Guid.NewGuid());

        // Act & Assert
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_WithSameId_GetHashCode_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new StubEntity(id);
        var entity2 = new StubEntity(id);

        // Act & Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    [Fact]
    public void Entity_ComparedToNull_ShouldNotBeEqual()
    {
        // Arrange
        var entity = new StubEntity(Guid.NewGuid());

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
    }

    // -----------------------------------------------------------------------
    // Id
    // -----------------------------------------------------------------------

    [Fact]
    public void Entity_Id_ShouldMatchConstructorArgument()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var entity = new StubEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    // -----------------------------------------------------------------------
    // Describe (virtual — default implementation)
    // -----------------------------------------------------------------------

    [Fact]
    public void Entity_Describe_DefaultImplementation_ShouldReturnNonNullString()
    {
        // Arrange
        var entity = new StubEntity(Guid.NewGuid());

        // Act
        var description = entity.Describe();

        // Assert
        description.Should().NotBeNull();
    }

    [Fact]
    public void Entity_Describe_DefaultImplementation_ShouldContainTypeName()
    {
        // Arrange
        var entity = new StubEntity(Guid.NewGuid());

        // Act
        var description = entity.Describe();

        // Assert
        description.Should().Contain(nameof(StubEntity));
    }

    // -----------------------------------------------------------------------
    // Describe (override in subclass)
    // -----------------------------------------------------------------------

    [Fact]
    public void Entity_Describe_WhenOverridden_ShouldReturnSubclassValue()
    {
        // Arrange
        var entity = new AnotherStubEntity(Guid.NewGuid(), "TestLabel");

        // Act
        var description = entity.Describe();

        // Assert
        description.Should().Be("AnotherStubEntity(TestLabel)");
    }
}
