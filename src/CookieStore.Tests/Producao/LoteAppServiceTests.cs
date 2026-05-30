using CookieStore.Application.Producao;
using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute;

namespace CookieStore.Tests.Producao;

public class LoteAppServiceTests
{
    private readonly ILoteRepository _lotes = Substitute.For<ILoteRepository>();
    private readonly LoteFactory _factory = new();
    private readonly LoteAppService _sut;

    public LoteAppServiceTests() => _sut = new LoteAppService(_lotes, _factory);

    // -----------------------------------------------------------------------
    // RegisterAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task LoteAppService_RegisterAsync_WithValidData_ShouldCallAddAsync()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());

        // Act
        await _sut.RegisterAsync(saborId, yield: 100, totalCost: 200.00m);

        // Assert
        await _lotes.Received(1).AddAsync(Arg.Any<Lote>());
    }

    [Fact]
    public async Task LoteAppService_RegisterAsync_ShouldReturnGeneratedLoteId()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());

        // Act
        var id = await _sut.RegisterAsync(saborId, yield: 100, totalCost: 200.00m);

        // Assert
        id.Should().NotBeNull();
        id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LoteAppService_RegisterAsync_WithZeroYield_ShouldThrowDomainException()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());

        // Act
        var act = async () => await _sut.RegisterAsync(saborId, yield: 0, totalCost: 200.00m);

        // Assert
        await act.Should().ThrowAsync<CookieStore.Domain.DomainException>();
    }

    [Fact]
    public async Task LoteAppService_RegisterAsync_WithNegativeCost_ShouldThrowDomainException()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());

        // Act
        var act = async () => await _sut.RegisterAsync(saborId, yield: 100, totalCost: -1.00m);

        // Assert
        await act.Should().ThrowAsync<CookieStore.Domain.DomainException>();
    }

    // -----------------------------------------------------------------------
    // GetByPeriodAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task LoteAppService_GetByPeriodAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var saborId = new SaborId(Guid.NewGuid());
        var factory = new LoteFactory();
        var lote = factory.Create(saborId, yield: 100, totalCost: 200.00m);

        _lotes.GetByPeriodAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
              .Returns([lote]);

        // Act
        var result = (await _sut.GetByPeriodAsync(DateTime.MinValue, DateTime.MaxValue)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].SaborId.Should().Be(saborId.Value);
        result[0].Yield.Should().Be(100);
        result[0].TotalCost.Should().Be(200.00m);
        result[0].CostPerUnit.Should().Be(2.00m);
    }
}
