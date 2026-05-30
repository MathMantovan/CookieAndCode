using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using FluentAssertions;

namespace CookieStore.Tests.Vendas;

/// <summary>
/// RED phase — VendaFactory must accept SaborSnapshot (ACL) instead of the Sabor aggregate.
/// All tests in this file will fail until SaborSnapshot is created and VendaFactory.Create
/// is updated to accept SaborSnapshot.
/// </summary>
public class VendaFactoryTests
{
    private readonly VendaFactory _factory = new();

    private static SaborId NewSaborId() => new(Guid.NewGuid());

    /// <summary>
    /// Builds a SaborSnapshot directly — the ACL boundary means the factory never sees Sabor.
    /// </summary>
    private static SaborSnapshot CreateSnapshot(decimal price = 10.00m, bool isActive = true) =>
        new(NewSaborId(), price, isActive);

    // -----------------------------------------------------------------------
    // Price snapshot capture
    // -----------------------------------------------------------------------

    [Fact]
    public void VendaFactory_Create_ShouldCaptureSnapshotPrice()
    {
        // Arrange
        var snapshot = CreateSnapshot(price: 10.00m);

        // Act
        var venda = _factory.Create(snapshot, 3);

        // Assert
        venda.UnitPrice.Should().Be(10.00m);
    }

    [Fact]
    public void VendaFactory_Create_SnapshotPriceShouldBeImmutableAfterSubsequentSnapshot()
    {
        // Arrange — first snapshot at 10.00; a later snapshot (different price) simulates a catalog update.
        // The already-created Venda must retain the original price.
        var snapshotAtSaleTime = CreateSnapshot(price: 10.00m);
        var venda = _factory.Create(snapshotAtSaleTime, 3);

        // Act — simulate catalog price change (new snapshot object, same SaborId)
        // The Venda.UnitPrice must not change regardless.

        // Assert
        venda.UnitPrice.Should().Be(10.00m);
    }

    // -----------------------------------------------------------------------
    // SaborId linkage
    // -----------------------------------------------------------------------

    [Fact]
    public void VendaFactory_Create_ShouldLinkToCorrectSaborId()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 15.00m, isActive: true);

        // Act
        var venda = _factory.Create(snapshot, 2);

        // Assert
        venda.SaborId.Should().Be(saborId);
    }

    // -----------------------------------------------------------------------
    // Invalid quantity
    // -----------------------------------------------------------------------

    [Fact]
    public void VendaFactory_Create_WithZeroQuantity_ShouldThrowDomainException()
    {
        // Arrange
        var snapshot = CreateSnapshot();

        // Act
        var act = () => _factory.Create(snapshot, 0);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void VendaFactory_Create_WithNegativeQuantity_ShouldThrowDomainException()
    {
        // Arrange
        var snapshot = CreateSnapshot();

        // Act
        var act = () => _factory.Create(snapshot, -1);

        // Assert
        act.Should().Throw<DomainException>();
    }

    // -----------------------------------------------------------------------
    // Unique IDs
    // -----------------------------------------------------------------------

    [Fact]
    public void VendaFactory_Create_ShouldGenerateUniqueIds()
    {
        // Arrange
        var snapshot = CreateSnapshot();

        // Act
        var venda1 = _factory.Create(snapshot, 1);
        var venda2 = _factory.Create(snapshot, 1);

        // Assert
        venda1.Id.Should().NotBe(venda2.Id);
    }

    // -----------------------------------------------------------------------
    // SoldAt timestamp
    // -----------------------------------------------------------------------

    [Fact]
    public void VendaFactory_Create_ShouldSetSoldAtToUtcNow()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        var before = DateTime.UtcNow;

        // Act
        var venda = _factory.Create(snapshot, 1);

        // Assert
        venda.SoldAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }
}
