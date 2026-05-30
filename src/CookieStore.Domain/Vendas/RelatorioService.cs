using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>
/// Domain Service for sales reporting (DDD: Domain Service justified because profit-per-flavor logic
/// crosses the Vendas and Producao BCs — it belongs to neither aggregate individually).
/// Depends only on domain interfaces — no EF Core, no infrastructure concerns.
/// </summary>
public class RelatorioService : IRelatorioService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly ILoteRepository _loteRepository;

    public RelatorioService(IVendaRepository vendaRepository, ILoteRepository loteRepository)
    {
        _vendaRepository = vendaRepository;
        _loteRepository = loteRepository;
    }

    public async Task<IEnumerable<LucroPorSabor>> GetProfitBySaborAsync(DateTime from, DateTime to)
    {
        var vendas = await _vendaRepository.GetByPeriodAsync(from, to);
        var lotes = await _loteRepository.GetByPeriodAsync(from, to);

        var revenueByFlavor = vendas
            .GroupBy(v => v.SaborId)
            .ToDictionary(g => g.Key, g => g.Sum(v => v.Total));

        var costByFlavor = lotes
            .GroupBy(l => l.SaborId)
            .ToDictionary(g => g.Key, g => g.Sum(l => l.TotalCost.Value));

        var allSaborIds = revenueByFlavor.Keys.Union(costByFlavor.Keys);

        return allSaborIds.Select(saborId =>
        {
            var revenue = revenueByFlavor.GetValueOrDefault(saborId, 0m);
            var cost = costByFlavor.GetValueOrDefault(saborId, 0m);
            return new LucroPorSabor(saborId, revenue, cost, Margin: revenue - cost);
        });
    }

    public async Task<ResumoVendas> GetSummaryAsync(DateTime from, DateTime to)
    {
        var profits = (await GetProfitBySaborAsync(from, to)).ToList();

        var totalRevenue = profits.Sum(p => p.Revenue);
        var totalCost = profits.Sum(p => p.Cost);

        return new ResumoVendas(totalRevenue, totalCost, GlobalMargin: totalRevenue - totalCost);
    }
}
