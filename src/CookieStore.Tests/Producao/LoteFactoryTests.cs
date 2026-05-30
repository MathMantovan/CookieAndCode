using CookieStore.Domain;
using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.Producao;

public class LoteFactoryTests
{
    private readonly LoteFactory _factory = new();

    private static SaborId AnySaborId() => new(Guid.NewGuid());

    [Fact]
    public void LoteFactory_Create_WithValidData_ShouldCreateLote()
    {
        var saborId = AnySaborId();

        var lote = _factory.Create(saborId, yield: 100, totalCost: 200.00m);

        lote.Should().NotBeNull();
        lote.SaborId.Should().Be(saborId);
        lote.Yield.Value.Should().Be(100);
        lote.TotalCost.Value.Should().Be(200.00m);
    }

    [Fact]
    public void LoteFactory_Create_WithZeroYield_ShouldThrowDomainException()
    {
        var act = () => _factory.Create(AnySaborId(), yield: 0, totalCost: 200.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void LoteFactory_Create_WithNegativeYield_ShouldThrowDomainException()
    {
        var act = () => _factory.Create(AnySaborId(), yield: -1, totalCost: 200.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void LoteFactory_Create_WithZeroTotalCost_ShouldThrowDomainException()
    {
        var act = () => _factory.Create(AnySaborId(), yield: 100, totalCost: 0m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void LoteFactory_Create_WithNegativeTotalCost_ShouldThrowDomainException()
    {
        var act = () => _factory.Create(AnySaborId(), yield: 100, totalCost: -50.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void LoteFactory_Create_ShouldGenerateUniqueIds()
    {
        var saborId = AnySaborId();

        var lote1 = _factory.Create(saborId, yield: 100, totalCost: 200.00m);
        var lote2 = _factory.Create(saborId, yield: 100, totalCost: 200.00m);

        lote1.Id.Should().NotBe(lote2.Id);
    }

    [Fact]
    public void LoteFactory_Create_ShouldSetProducedAtToUtcNow()
    {
        var before = DateTime.UtcNow;

        var lote = _factory.Create(AnySaborId(), yield: 100, totalCost: 200.00m);

        lote.ProducedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void LoteFactory_Create_ShouldLinkToCorrectSabor()
    {
        var saborId = AnySaborId();

        var lote = _factory.Create(saborId, yield: 100, totalCost: 200.00m);

        lote.SaborId.Should().Be(saborId);
    }
}
