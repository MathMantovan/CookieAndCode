using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.Producao;

public class LoteTests
{
    private readonly LoteFactory _factory = new();

    private static SaborId AnySaborId() => new(Guid.NewGuid());

    [Fact]
    public void Lote_CostPerUnit_ShouldBeTotalCostDividedByYield()
    {
        var lote = _factory.Create(AnySaborId(), yield: 100, totalCost: 200.00m);

        lote.CostPerUnit.Should().Be(2.00m);
    }

    [Fact]
    public void Lote_CostPerUnit_ShouldReflectExactDivision()
    {
        var lote = _factory.Create(AnySaborId(), yield: 3, totalCost: 10.00m);

        lote.CostPerUnit.Should().BeApproximately(3.333m, precision: 0.001m);
    }

    [Fact]
    public void Lote_Properties_ShouldBeSetCorrectly()
    {
        var saborId = AnySaborId();

        var lote = _factory.Create(saborId, yield: 100, totalCost: 200.00m);

        lote.Id.Should().NotBeEmpty();
        lote.SaborId.Should().Be(saborId);
        lote.Yield.Value.Should().Be(100);
        lote.TotalCost.Value.Should().Be(200.00m);
        lote.ProducedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
