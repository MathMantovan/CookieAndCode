using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using FluentAssertions;

namespace CookieStore.Tests.Vendas;

public class RelatorioServiceTests
{
    private static readonly DateTime From = DateTime.UtcNow.AddDays(-30);
    private static readonly DateTime To = DateTime.UtcNow.AddDays(1);

    private readonly VendaFactory _vendaFactory = new();
    private readonly LoteFactory _loteFactory = new();

    // -----------------------------------------------------------------------
    // In-memory stubs — pure domain test, no mocking framework (TDD principle:
    // domain tests are isolated from infrastructure)
    // -----------------------------------------------------------------------

    private static RelatorioService Build(IEnumerable<Venda> vendas, IEnumerable<Lote> lotes) =>
        new(new VendaStub(vendas), new LoteStub(lotes));

    private sealed class VendaStub(IEnumerable<Venda> vendas) : IVendaRepository
    {
        public Task<IEnumerable<Venda>> GetByPeriodAsync(DateTime from, DateTime to) =>
            Task.FromResult(vendas);

        public Task<IEnumerable<Venda>> GetBySaborAsync(SaborId saborId) =>
            Task.FromResult(vendas.Where(v => v.SaborId == saborId));

        public Task AddAsync(Venda venda) => Task.CompletedTask;
    }

    private sealed class LoteStub(IEnumerable<Lote> lotes) : ILoteRepository
    {
        public Task<IEnumerable<Lote>> GetByPeriodAsync(DateTime from, DateTime to) =>
            Task.FromResult(lotes);

        public Task<IEnumerable<Lote>> GetBySaborAsync(SaborId saborId) =>
            Task.FromResult(lotes.Where(l => l.SaborId == saborId));

        public Task AddAsync(Lote lote) => Task.CompletedTask;
    }

    // -----------------------------------------------------------------------
    // GetProfitBySaborAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task RelatorioService_GetProfitBySabor_WithSalesAndBatches_ShouldCalculateMarginCorrectly()
    {
        // Arrange
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        var snapshot = new SaborSnapshot(sabor.SaborId, sabor.CatalogPrice.Value, sabor.IsActive);
        var venda = _vendaFactory.Create(snapshot, quantity: 3); // Revenue = 30
        var lote = _loteFactory.Create(sabor.SaborId, yield: 10, totalCost: 20.00m); // Cost = 20

        var service = Build([venda], [lote]);

        // Act
        var result = (await service.GetProfitBySaborAsync(From, To)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].SaborId.Should().Be(sabor.SaborId);
        result[0].Revenue.Should().Be(30.00m);
        result[0].Cost.Should().Be(20.00m);
        result[0].Margin.Should().Be(10.00m);
    }

    [Fact]
    public async Task RelatorioService_GetProfitBySabor_WithNoSalesInPeriod_RevenueShouldBeZero()
    {
        // Arrange
        var sabor = new Sabor("Morango", PrecoTabela.Create(8.00m));
        var lote = _loteFactory.Create(sabor.SaborId, yield: 50, totalCost: 100.00m);

        var service = Build([], [lote]);

        // Act
        var result = (await service.GetProfitBySaborAsync(From, To)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Revenue.Should().Be(0m);
        result[0].Cost.Should().Be(100.00m);
        result[0].Margin.Should().Be(-100.00m);
    }

    [Fact]
    public async Task RelatorioService_GetProfitBySabor_WithNoBatchesInPeriod_CostShouldBeZero()
    {
        // Arrange
        var sabor = new Sabor("Baunilha", PrecoTabela.Create(12.00m));
        var snapshot = new SaborSnapshot(sabor.SaborId, sabor.CatalogPrice.Value, sabor.IsActive);
        var venda = _vendaFactory.Create(snapshot, quantity: 5); // Revenue = 60

        var service = Build([venda], []);

        // Act
        var result = (await service.GetProfitBySaborAsync(From, To)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Revenue.Should().Be(60.00m);
        result[0].Cost.Should().Be(0m);
        result[0].Margin.Should().Be(60.00m);
    }

    [Fact]
    public async Task RelatorioService_GetProfitBySabor_WithMultipleSabores_ShouldGroupBySaborId()
    {
        // Arrange
        var saborA = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        var saborB = new Sabor("Morango", PrecoTabela.Create(8.00m));
        var snapshotA = new SaborSnapshot(saborA.SaborId, saborA.CatalogPrice.Value, saborA.IsActive);
        var snapshotB = new SaborSnapshot(saborB.SaborId, saborB.CatalogPrice.Value, saborB.IsActive);

        var vendaA = _vendaFactory.Create(snapshotA, quantity: 2); // Revenue = 20
        var vendaB = _vendaFactory.Create(snapshotB, quantity: 3); // Revenue = 24

        var loteA = _loteFactory.Create(saborA.SaborId, yield: 10, totalCost: 15.00m);
        var loteB = _loteFactory.Create(saborB.SaborId, yield: 20, totalCost: 30.00m);

        var service = Build([vendaA, vendaB], [loteA, loteB]);

        // Act
        var result = (await service.GetProfitBySaborAsync(From, To))
            .OrderBy(r => r.Revenue)
            .ToList();

        // Assert — sorted ascending by Revenue: saborA (20) first, saborB (24) second
        result.Should().HaveCount(2);
        result[0].SaborId.Should().Be(saborA.SaborId);
        result[0].Revenue.Should().Be(20.00m);
        result[1].SaborId.Should().Be(saborB.SaborId);
        result[1].Revenue.Should().Be(24.00m);
    }

    [Fact]
    public async Task RelatorioService_GetProfitBySabor_WithNoData_ShouldReturnEmptyCollection()
    {
        // Arrange
        var service = Build([], []);

        // Act
        var result = await service.GetProfitBySaborAsync(From, To);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task RelatorioService_GetProfitBySabor_MultipleSalesForSameSabor_ShouldSumRevenue()
    {
        // Arrange
        var sabor = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        var snapshot = new SaborSnapshot(sabor.SaborId, sabor.CatalogPrice.Value, sabor.IsActive);
        var venda1 = _vendaFactory.Create(snapshot, quantity: 2); // 20
        var venda2 = _vendaFactory.Create(snapshot, quantity: 3); // 30

        var service = Build([venda1, venda2], []);

        // Act
        var result = (await service.GetProfitBySaborAsync(From, To)).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Revenue.Should().Be(50.00m);
    }

    // -----------------------------------------------------------------------
    // GetSummaryAsync
    // -----------------------------------------------------------------------

    [Fact]
    public async Task RelatorioService_GetSummary_ShouldAggregateAllSabores()
    {
        // Arrange
        var saborA = new Sabor("Chocolate", PrecoTabela.Create(10.00m));
        var saborB = new Sabor("Morango", PrecoTabela.Create(5.00m));
        var snapshotA = new SaborSnapshot(saborA.SaborId, saborA.CatalogPrice.Value, saborA.IsActive);
        var snapshotB = new SaborSnapshot(saborB.SaborId, saborB.CatalogPrice.Value, saborB.IsActive);

        var vendaA = _vendaFactory.Create(snapshotA, quantity: 2); // 20
        var vendaB = _vendaFactory.Create(snapshotB, quantity: 4); // 20

        var loteA = _loteFactory.Create(saborA.SaborId, yield: 10, totalCost: 15.00m);
        var loteB = _loteFactory.Create(saborB.SaborId, yield: 10, totalCost: 10.00m);

        var service = Build([vendaA, vendaB], [loteA, loteB]);

        // Act
        var summary = await service.GetSummaryAsync(From, To);

        // Assert
        summary.TotalRevenue.Should().Be(40.00m);
        summary.TotalCost.Should().Be(25.00m);
        summary.GlobalMargin.Should().Be(15.00m);
    }

    [Fact]
    public async Task RelatorioService_GetSummary_WithNoData_ShouldReturnZeroes()
    {
        // Arrange
        var service = Build([], []);

        // Act
        var summary = await service.GetSummaryAsync(From, To);

        // Assert
        summary.TotalRevenue.Should().Be(0m);
        summary.TotalCost.Should().Be(0m);
        summary.GlobalMargin.Should().Be(0m);
    }
}
