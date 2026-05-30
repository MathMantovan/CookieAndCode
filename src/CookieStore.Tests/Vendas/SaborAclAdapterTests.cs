using CookieStore.Application.Vendas;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CookieStore.Tests.Vendas;

/// <summary>
/// RED phase — SaborAclAdapter translates between ISaborRepository (SharedKernel) and
/// ISaborAcl (Vendas BC). These tests will fail until SaborSnapshot, ISaborAcl, and
/// SaborAclAdapter are created in their respective projects.
/// </summary>
public class SaborAclAdapterTests
{
    private readonly ISaborRepository _saborRepository = Substitute.For<ISaborRepository>();
    private readonly SaborAclAdapter _sut;

    public SaborAclAdapterTests() =>
        _sut = new SaborAclAdapter(_saborRepository);

    // -----------------------------------------------------------------------
    // GetSnapshotAsync — sabor exists
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_WithExistingSabor_ShouldReturnSnapshot()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(12.50m));
        _saborRepository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        var snapshot = await _sut.GetSnapshotAsync(saborId);

        // Assert
        snapshot.Should().NotBeNull();
    }

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_ShouldMapSaborIdCorrectly()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Morango", PrecoTabela.Create(8.00m));
        _saborRepository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        var snapshot = await _sut.GetSnapshotAsync(saborId);

        // Assert
        snapshot!.Id.Should().Be(sabor.SaborId);
    }

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_ShouldMapCurrentPriceCorrectly()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Baunilha", PrecoTabela.Create(9.99m));
        _saborRepository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        var snapshot = await _sut.GetSnapshotAsync(saborId);

        // Assert
        snapshot!.CurrentPrice.Should().Be(9.99m);
    }

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_ShouldMapIsActiveCorrectly()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Limao", PrecoTabela.Create(7.00m));
        // sabor is active by default after construction
        _saborRepository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        var snapshot = await _sut.GetSnapshotAsync(saborId);

        // Assert
        snapshot!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_WhenSaborIsInactive_ShouldMapIsActiveFalse()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Passas", PrecoTabela.Create(6.50m));
        sabor.Deactivate();
        _saborRepository.GetByIdAsync(saborId).Returns(sabor);

        // Act
        var snapshot = await _sut.GetSnapshotAsync(saborId);

        // Assert
        snapshot!.IsActive.Should().BeFalse();
    }

    // -----------------------------------------------------------------------
    // GetSnapshotAsync — sabor not found
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_WithNonExistentSabor_ShouldReturnNull()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        _saborRepository.GetByIdAsync(saborId).ReturnsNull();

        // Act
        var snapshot = await _sut.GetSnapshotAsync(saborId);

        // Assert
        snapshot.Should().BeNull();
    }

    // -----------------------------------------------------------------------
    // Repository delegation
    // -----------------------------------------------------------------------

    [Fact]
    public async Task SaborAclAdapter_GetSnapshotAsync_ShouldCallGetByIdAsyncExactlyOnce()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        _saborRepository.GetByIdAsync(saborId).ReturnsNull();

        // Act
        await _sut.GetSnapshotAsync(saborId);

        // Assert
        await _saborRepository.Received(1).GetByIdAsync(saborId);
    }
}
