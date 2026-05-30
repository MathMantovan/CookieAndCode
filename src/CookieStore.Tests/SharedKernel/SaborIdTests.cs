using CookieStore.Domain.SharedKernel;
using FluentAssertions;

namespace CookieStore.Tests.SharedKernel;

public class SaborIdTests
{
    [Fact]
    public void SaborId_WithSameGuid_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var id1 = new SaborId(guid);
        var id2 = new SaborId(guid);

        id1.Should().Be(id2);
    }

    [Fact]
    public void SaborId_WithDifferentGuids_ShouldNotBeEqual()
    {
        var id1 = new SaborId(Guid.NewGuid());
        var id2 = new SaborId(Guid.NewGuid());

        id1.Should().NotBe(id2);
    }

    [Fact]
    public void SaborId_Value_ShouldReturnProvidedGuid()
    {
        var guid = Guid.NewGuid();
        var id = new SaborId(guid);

        id.Value.Should().Be(guid);
    }
}
