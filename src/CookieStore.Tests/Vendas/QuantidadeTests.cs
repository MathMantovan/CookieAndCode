using CookieStore.Domain;
using CookieStore.Domain.Vendas;
using FluentAssertions;

namespace CookieStore.Tests.Vendas;

public class QuantidadeTests
{
    [Fact]
    public void Quantidade_WithPositiveValue_ShouldCreate()
    {
        var qty = Quantidade.Create(5);

        qty.Value.Should().Be(5);
    }

    [Fact]
    public void Quantidade_WithZeroValue_ShouldThrowDomainException()
    {
        var act = () => Quantidade.Create(0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Quantidade_WithNegativeValue_ShouldThrowDomainException()
    {
        var act = () => Quantidade.Create(-1);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Quantidade_WithSameValue_ShouldBeEqual()
    {
        var qty1 = Quantidade.Create(3);
        var qty2 = Quantidade.Create(3);

        qty1.Should().Be(qty2);
    }

    [Fact]
    public void Quantidade_WithSameValue_EqualityOperator_ShouldBeTrue()
    {
        var qty1 = Quantidade.Create(3);
        var qty2 = Quantidade.Create(3);

        (qty1 == qty2).Should().BeTrue();
    }

    [Fact]
    public void Quantidade_WithDifferentValues_ShouldNotBeEqual()
    {
        var qty1 = Quantidade.Create(3);
        var qty2 = Quantidade.Create(5);

        qty1.Should().NotBe(qty2);
    }

    [Fact]
    public void Quantidade_WithDifferentValues_InequalityOperator_ShouldBeTrue()
    {
        var qty1 = Quantidade.Create(3);
        var qty2 = Quantidade.Create(5);

        (qty1 != qty2).Should().BeTrue();
    }

    [Fact]
    public void Quantidade_WithSameValue_GetHashCode_ShouldBeEqual()
    {
        var qty1 = Quantidade.Create(3);
        var qty2 = Quantidade.Create(3);

        qty1.GetHashCode().Should().Be(qty2.GetHashCode());
    }
}
