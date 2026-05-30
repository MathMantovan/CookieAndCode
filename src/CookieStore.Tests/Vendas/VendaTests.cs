using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using FluentAssertions;

namespace CookieStore.Tests.Vendas;

/// <summary>
/// RED phase — VendaFactory.Create now accepts SaborSnapshot (ACL boundary).
/// Tests will fail until VendaFactory is updated.
/// </summary>
public class VendaTests
{
    private readonly VendaFactory _factory = new();

    /// <summary>Builds a SaborSnapshot — the factory no longer depends on the Sabor aggregate.</summary>
    private static SaborSnapshot CreateSnapshot(decimal price = 10.00m) =>
        new(new SaborId(Guid.NewGuid()), price, isActive: true);

    // -----------------------------------------------------------------------
    // Total computation
    // -----------------------------------------------------------------------

    [Fact]
    public void Venda_Total_ShouldBeUnitPriceTimesQuantity()
    {
        // Arrange
        var snapshot = CreateSnapshot(price: 10.00m);

        // Act
        var venda = _factory.Create(snapshot, 3);

        // Assert
        venda.Total.Should().Be(30.00m);
    }

    [Fact]
    public void Venda_Total_ShouldReflectActualQuantityAndPrice()
    {
        // Arrange
        var snapshot = CreateSnapshot(price: 7.50m);

        // Act
        var venda = _factory.Create(snapshot, 4);

        // Assert
        venda.Total.Should().Be(30.00m);
    }

    // -----------------------------------------------------------------------
    // Property correctness
    // -----------------------------------------------------------------------

    [Fact]
    public void Venda_Properties_ShouldBeSetCorrectly()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 10.00m, isActive: true);

        // Act
        var venda = _factory.Create(snapshot, 2);

        // Assert
        venda.Id.Should().NotBeEmpty();
        venda.SaborId.Should().Be(saborId);
        venda.Quantity.Value.Should().Be(2);
        venda.UnitPrice.Should().Be(10.00m);
        venda.SoldAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
