using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.SharedKernel;

public class SaborTests
{
    private static PrecoTabela ValidPrice() => PrecoTabela.Create(10.00m);

    [Fact]
    public void Sabor_WithValidData_ShouldCreate()
    {
        var sabor = new Sabor("Chocolate", ValidPrice());

        sabor.Name.Should().Be("Chocolate");
        sabor.CatalogPrice.Value.Should().Be(10.00m);
        sabor.IsActive.Should().BeTrue();
        sabor.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Sabor_SaborId_ShouldMatchUnderlyingId()
    {
        var sabor = new Sabor("Chocolate", ValidPrice());

        sabor.SaborId.Value.Should().Be(sabor.Id);
    }

    [Fact]
    public void Sabor_WithEmptyName_ShouldThrowDomainException()
    {
        var act = () => new Sabor("", ValidPrice());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Sabor_WithWhitespaceName_ShouldThrowDomainException()
    {
        var act = () => new Sabor("   ", ValidPrice());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Sabor_WithZeroPrice_ShouldThrowDomainException()
    {
        var act = () => new Sabor("Chocolate", PrecoTabela.Create(0m));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Sabor_WithNegativePrice_ShouldThrowDomainException()
    {
        var act = () => new Sabor("Chocolate", PrecoTabela.Create(-1m));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Sabor_UpdatePrice_WithPositiveValue_ShouldUpdateCatalogPrice()
    {
        var sabor = new Sabor("Chocolate", ValidPrice());

        sabor.UpdatePrice(PrecoTabela.Create(20.00m));

        sabor.CatalogPrice.Value.Should().Be(20.00m);
    }

    [Fact]
    public void Sabor_UpdatePrice_WithZeroValue_ShouldThrowDomainException()
    {
        var sabor = new Sabor("Chocolate", ValidPrice());

        var act = () => sabor.UpdatePrice(PrecoTabela.Create(0m));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Sabor_Deactivate_WhenActive_ShouldSetIsActiveFalse()
    {
        var sabor = new Sabor("Chocolate", ValidPrice());

        sabor.Deactivate();

        sabor.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Sabor_Deactivate_WhenAlreadyInactive_ShouldNotThrow()
    {
        var sabor = new Sabor("Chocolate", ValidPrice());
        sabor.Deactivate();

        var act = () => sabor.Deactivate();

        act.Should().NotThrow();
    }

    [Fact]
    public void Sabor_EachInstance_ShouldHaveUniqueId()
    {
        var sabor1 = new Sabor("Chocolate", ValidPrice());
        var sabor2 = new Sabor("Morango", ValidPrice());

        sabor1.Id.Should().NotBe(sabor2.Id);
    }
}
