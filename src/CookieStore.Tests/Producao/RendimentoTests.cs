using CookieStore.Domain;
using CookieStore.Domain.Producao;
using FluentAssertions;

namespace CookieStore.Tests.Producao;

public class RendimentoTests
{
    [Fact]
    public void Rendimento_WithPositiveValue_ShouldCreate()
    {
        var rendimento = Rendimento.Create(100);

        rendimento.Value.Should().Be(100);
    }

    [Fact]
    public void Rendimento_WithZeroValue_ShouldThrowDomainException()
    {
        // Zero yield would cause division by zero in CostPerUnit
        var act = () => Rendimento.Create(0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Rendimento_WithNegativeValue_ShouldThrowDomainException()
    {
        var act = () => Rendimento.Create(-1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Rendimento_WithSameValue_ShouldBeEqual()
    {
        var r1 = Rendimento.Create(100);
        var r2 = Rendimento.Create(100);

        r1.Should().Be(r2);
    }

    [Fact]
    public void Rendimento_WithSameValue_EqualityOperator_ShouldBeTrue()
    {
        var r1 = Rendimento.Create(100);
        var r2 = Rendimento.Create(100);

        (r1 == r2).Should().BeTrue();
    }

    [Fact]
    public void Rendimento_WithDifferentValues_ShouldNotBeEqual()
    {
        var r1 = Rendimento.Create(100);
        var r2 = Rendimento.Create(200);

        r1.Should().NotBe(r2);
    }

    [Fact]
    public void Rendimento_WithDifferentValues_InequalityOperator_ShouldBeTrue()
    {
        var r1 = Rendimento.Create(100);
        var r2 = Rendimento.Create(200);

        (r1 != r2).Should().BeTrue();
    }

    [Fact]
    public void Rendimento_WithSameValue_GetHashCode_ShouldBeEqual()
    {
        var r1 = Rendimento.Create(100);
        var r2 = Rendimento.Create(100);

        r1.GetHashCode().Should().Be(r2.GetHashCode());
    }
}
