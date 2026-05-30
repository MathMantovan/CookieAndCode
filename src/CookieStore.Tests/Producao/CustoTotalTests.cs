using CookieStore.Domain;
using CookieStore.Domain.Producao;
using FluentAssertions;

namespace CookieStore.Tests.Producao;

public class CustoTotalTests
{
    [Fact]
    public void CustoTotal_WithPositiveValue_ShouldCreate()
    {
        var cost = CustoTotal.Create(200.00m);

        cost.Value.Should().Be(200.00m);
    }

    [Fact]
    public void CustoTotal_WithZeroValue_ShouldThrowDomainException()
    {
        var act = () => CustoTotal.Create(0m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CustoTotal_WithNegativeValue_ShouldThrowDomainException()
    {
        var act = () => CustoTotal.Create(-50.00m);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CustoTotal_WithSameValue_ShouldBeEqual()
    {
        var c1 = CustoTotal.Create(200.00m);
        var c2 = CustoTotal.Create(200.00m);

        c1.Should().Be(c2);
    }

    [Fact]
    public void CustoTotal_WithSameValue_EqualityOperator_ShouldBeTrue()
    {
        var c1 = CustoTotal.Create(200.00m);
        var c2 = CustoTotal.Create(200.00m);

        (c1 == c2).Should().BeTrue();
    }

    [Fact]
    public void CustoTotal_WithDifferentValues_ShouldNotBeEqual()
    {
        var c1 = CustoTotal.Create(200.00m);
        var c2 = CustoTotal.Create(300.00m);

        c1.Should().NotBe(c2);
    }

    [Fact]
    public void CustoTotal_WithDifferentValues_InequalityOperator_ShouldBeTrue()
    {
        var c1 = CustoTotal.Create(200.00m);
        var c2 = CustoTotal.Create(300.00m);

        (c1 != c2).Should().BeTrue();
    }

    [Fact]
    public void CustoTotal_WithSameValue_GetHashCode_ShouldBeEqual()
    {
        var c1 = CustoTotal.Create(200.00m);
        var c2 = CustoTotal.Create(200.00m);

        c1.GetHashCode().Should().Be(c2.GetHashCode());
    }
}
