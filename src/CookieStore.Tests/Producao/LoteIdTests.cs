using CookieStore.Domain.Producao;
using FluentAssertions;

namespace CookieStore.Tests.Producao;

public class LoteIdTests
{
    [Fact]
    public void LoteId_WithSameGuid_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var id1 = new LoteId(guid);
        var id2 = new LoteId(guid);

        id1.Should().Be(id2);
    }

    [Fact]
    public void LoteId_WithDifferentGuids_ShouldNotBeEqual()
    {
        var id1 = new LoteId(Guid.NewGuid());
        var id2 = new LoteId(Guid.NewGuid());

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void LoteId_Value_ShouldReturnProvidedGuid()
    {
        var guid = Guid.NewGuid();
        var id = new LoteId(guid);

        id.Value.Should().Be(guid);
    }
}
