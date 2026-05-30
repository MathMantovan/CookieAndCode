using CookieStore.Domain.Vendas;
using FluentAssertions;

namespace CookieStore.Tests.Vendas;

public class VendaIdTests
{
    [Fact]
    public void VendaId_WithSameGuid_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var id1 = new VendaId(guid);
        var id2 = new VendaId(guid);

        id1.Should().Be(id2);
    }

    [Fact]
    public void VendaId_WithDifferentGuids_ShouldNotBeEqual()
    {
        var id1 = new VendaId(Guid.NewGuid());
        var id2 = new VendaId(Guid.NewGuid());

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void VendaId_Value_ShouldReturnProvidedGuid()
    {
        var guid = Guid.NewGuid();
        var id = new VendaId(guid);

        id.Value.Should().Be(guid);
    }
}
