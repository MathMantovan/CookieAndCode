using CookieStore.Application.Relatorio;
using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using FluentAssertions;

namespace CookieStore.Tests.Relatorio;

public class RelatorioAppServiceTests
{
    private readonly VendaFactory _vendaFactory = new();
    private readonly LoteFactory _loteFactory = new();

    private static readonly DateTime From = DateTime.UtcNow.AddDays(-30);
    private static readonly DateTime To = DateTime.UtcNow.AddDays(1);

    // In-memory stubs — RelatorioService is a concrete domain service;
    // tested via stubs to keep the test pure and avoid infrastructure coupling.
    private RelatorioAppService BuildService(IEnumerable<Venda> vendas, IEnumerable<Lote> lotes)
    {
        var domainService = new RelatorioService(new VendaStub(vendas), new LoteStub(lotes));
        return new RelatorioAppService(domainService);
    }

    private sealed class VendaStub(IEnumerable<Venda> vendas) : IVendaRepository
    {
        public Task<IEnumerable<Venda>> GetByPeriodAsync(DateTime from, DateTime to) => Task.FromResult(vendas);
        public Task<IEnumerable<Venda>> GetBySaborAsync(SaborId saborId) => Task.FromResult(vendas.Where(v => v.SaborId == saborId));
        public Task AddAsync(Venda venda) => Task.CompletedTask;
    }

    private sealed class LoteStub(IEnumerable<Lote> lotes) : ILoteRepository
    {
        public Task<IEnumerable<Lote>> GetByPeriodAsync(DateTime from, DateTime to) => Task.FromResult(lotes);
        public Task<IEnumerable<Lote>> GetBySaborAsync(SaborId saborId) => Task.FromResult(lotes.Where(l => l.SaborId == saborId));
        public Task AddAsync(Lote lote) => Task.CompletedTask;
    }

    // -----------------------------------------------------------------------
    // GetProfitBySaborAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task RelatorioAppService_GetProfitBySabor_ShouldReturnMappedDtos()
    {
        // Arrange
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        var snapshot = new SaborSnapshot(sabor.SaborId, sabor.CatalogPrice.Value, sabor.IsActive);
        var venda = _vendaFactory.Create(snapshot, quantity: 3); // Revenue = 30
        var lote = _loteFactory.Create(sabor.SaborId, yield: 10, totalCost: 20.00m); // Cost = 20

        var service = BuildService([venda], [lote]);

        // Act
        var result = (await service.GetProfitBySaborAsync(From, To)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].SaborId.Should().Be(sabor.SaborId.Value);
        result[0].Revenue.Should().Be(30.00m);
        result[0].Cost.Should().Be(20.00m);
        result[0].Margin.Should().Be(10.00m);
    }

    [Fact]
    public async Task RelatorioAppService_GetProfitBySabor_WithNoData_ShouldReturnEmptyList()
    {
        // Arrange
        var service = BuildService([], []);

        // Act
        var result = await service.GetProfitBySaborAsync(From, To);

        // Assert
        result.Should().BeEmpty();
    }

    // -----------------------------------------------------------------------
    // GetSummaryAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task RelatorioAppService_GetSummary_ShouldReturnMappedDto()
    {
        // Arrange
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        var snapshot = new SaborSnapshot(sabor.SaborId, sabor.CatalogPrice.Value, sabor.IsActive);
        var venda = _vendaFactory.Create(snapshot, quantity: 4); // Revenue = 40
        var lote = _loteFactory.Create(sabor.SaborId, yield: 10, totalCost: 25.00m); // Cost = 25

        var service = BuildService([venda], [lote]);

        // Act
        var summary = await service.GetSummaryAsync(From, To);

        // Assert
        summary.TotalRevenue.Should().Be(40.00m);
        summary.TotalCost.Should().Be(25.00m);
        summary.GlobalMargin.Should().Be(15.00m);
    }

    [Fact]
    public async Task RelatorioAppService_GetSummary_WithNoData_ShouldReturnZeroes()
    {
        // Arrange
        var service = BuildService([], []);

        // Act
        var summary = await service.GetSummaryAsync(From, To);

        // Assert
        summary.TotalRevenue.Should().Be(0m);
        summary.TotalCost.Should().Be(0m);
        summary.GlobalMargin.Should().Be(0m);
    }
}
