using CookieStore.Application.Vendas;
using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CookieStore.Tests.Vendas;

/// <summary>
/// RED phase — VendaAppService must depend on ISaborAcl (ACL) instead of ISaborRepository.
/// All tests in this file will fail until ISaborAcl is created and VendaAppService is updated
/// to accept ISaborAcl in its constructor and call GetSnapshotAsync.
/// </summary>
public class VendaAppServiceTests
{
    private readonly ISaborAcl _acl = Substitute.For<ISaborAcl>();
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();
    private readonly VendaFactory _factory = new();
    private readonly VendaAppService _sut;

    public VendaAppServiceTests() =>
        _sut = new VendaAppService(_acl, _vendas, _factory);

    // -----------------------------------------------------------------------
    // RegisterAsync — happy path
    // -----------------------------------------------------------------------

    [Fact]
    public async Task VendaAppService_RegisterAsync_WithExistingSnapshot_ShouldCallAddAsync()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 10.00m, isActive: true);
        _acl.GetSnapshotAsync(saborId).Returns(snapshot);

        // Act
        await _sut.RegisterAsync(saborId, quantity: 3);

        // Assert
        await _vendas.Received(1).AddAsync(Arg.Any<Venda>());
    }

    [Fact]
    public async Task VendaAppService_RegisterAsync_ShouldReturnGeneratedVendaId()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 10.00m, isActive: true);
        _acl.GetSnapshotAsync(saborId).Returns(snapshot);

        // Act
        var id = await _sut.RegisterAsync(saborId, quantity: 2);

        // Assert
        id.Should().NotBeNull();
        id.Value.Should().NotBeEmpty();
    }

    // -----------------------------------------------------------------------
    // RegisterAsync — snapshot not found
    // -----------------------------------------------------------------------

    [Fact]
    public async Task VendaAppService_RegisterAsync_WithNullSnapshot_ShouldThrowDomainException()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        _acl.GetSnapshotAsync(saborId).ReturnsNull();

        // Act
        var act = async () => await _sut.RegisterAsync(saborId, quantity: 2);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    // -----------------------------------------------------------------------
    // RegisterAsync — invalid quantity propagated from domain
    // -----------------------------------------------------------------------

    [Fact]
    public async Task VendaAppService_RegisterAsync_WithZeroQuantity_ShouldThrowDomainException()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 10.00m, isActive: true);
        _acl.GetSnapshotAsync(saborId).Returns(snapshot);

        // Act
        var act = async () => await _sut.RegisterAsync(saborId, quantity: 0);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
    }

    // -----------------------------------------------------------------------
    // RegisterAsync — ACL called exactly once
    // -----------------------------------------------------------------------

    [Fact]
    public async Task VendaAppService_RegisterAsync_ShouldCallGetSnapshotAsyncExactlyOnce()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 10.00m, isActive: true);
        _acl.GetSnapshotAsync(saborId).Returns(snapshot);

        // Act
        await _sut.RegisterAsync(saborId, quantity: 1);

        // Assert
        await _acl.Received(1).GetSnapshotAsync(saborId);
    }

    // -----------------------------------------------------------------------
    // GetByPeriodAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task VendaAppService_GetByPeriodAsync_ShouldReturnMappedDtos()
    {
        // Arrange — build a Venda via factory using SaborSnapshot (ACL boundary)
        var saborId = new SaborId(Guid.NewGuid());
        var snapshot = new SaborSnapshot(saborId, 10.00m, isActive: true);
        var factory = new VendaFactory();
        var venda = factory.Create(snapshot, quantity: 3);

        _vendas.GetByPeriodAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
               .Returns([venda]);

        // Act
        var result = (await _sut.GetByPeriodAsync(DateTime.MinValue, DateTime.MaxValue)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].SaborId.Should().Be(saborId.Value);
        result[0].Quantity.Should().Be(3);
        result[0].UnitPrice.Should().Be(10.00m);
        result[0].Total.Should().Be(30.00m);
    }
}
