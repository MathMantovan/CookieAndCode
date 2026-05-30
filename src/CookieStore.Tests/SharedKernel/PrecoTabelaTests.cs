using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.SharedKernel;

public class PrecoTabelaTests
{
    [Fact]
    public void PrecoTabela_WithPositiveValue_ShouldCreate()
    {
        var price = PrecoTabela.Create(10.00m);

        price.Value.Should().Be(10.00m);
    }

    [Fact]
    public void PrecoTabela_WithZeroValue_ShouldThrowDomainException()
    {
        var act = () => PrecoTabela.Create(0m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void PrecoTabela_WithNegativeValue_ShouldThrowDomainException()
    {
        var act = () => PrecoTabela.Create(-5.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void PrecoTabela_WithSameValue_ShouldBeEqual()
    {
        var price1 = PrecoTabela.Create(10.00m);
        var price2 = PrecoTabela.Create(10.00m);

        price1.Should().Be(price2);
    }

    [Fact]
    public void PrecoTabela_WithSameValue_EqualityOperator_ShouldBeTrue()
    {
        var price1 = PrecoTabela.Create(10.00m);
        var price2 = PrecoTabela.Create(10.00m);

        (price1 == price2).Should().BeTrue();
    }

    [Fact]
    public void PrecoTabela_WithDifferentValues_ShouldNotBeEqual()
    {
        var price1 = PrecoTabela.Create(10.00m);
        var price2 = PrecoTabela.Create(20.00m);

        price1.Should().NotBe(price2);
    }

    [Fact]
    public void PrecoTabela_WithDifferentValues_InequalityOperator_ShouldBeTrue()
    {
        var price1 = PrecoTabela.Create(10.00m);
        var price2 = PrecoTabela.Create(20.00m);

        (price1 != price2).Should().BeTrue();
    }

    [Fact]
    public void PrecoTabela_WithSameValue_GetHashCode_ShouldBeEqual()
    {
        var price1 = PrecoTabela.Create(10.00m);
        var price2 = PrecoTabela.Create(10.00m);

        price1.GetHashCode().Should().Be(price2.GetHashCode());
    }
}
