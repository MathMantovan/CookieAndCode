using CookieStore.Domain.Vendas;

namespace CookieStore.Application.Relatorio;

/// <summary>Application Service for reporting — delegates to IRelatorioService and maps results to DTOs.</summary>
public class RelatorioAppService
{
    private readonly IRelatorioService _relatorioService;

    public RelatorioAppService(IRelatorioService relatorioService) =>
        _relatorioService = relatorioService;

    public async Task<IEnumerable<LucroPorSaborDto>> GetProfitBySaborAsync(DateTime from, DateTime to)
    {
        var profits = await _relatorioService.GetProfitBySaborAsync(from, to);
        return profits.Select(p => new LucroPorSaborDto(p.SaborId.Value, p.Revenue, p.Cost, p.Margin));
    }

    public async Task<ResumoVendasDto> GetSummaryAsync(DateTime from, DateTime to)
    {
        var summary = await _relatorioService.GetSummaryAsync(from, to);
        return new ResumoVendasDto(summary.TotalRevenue, summary.TotalCost, summary.GlobalMargin);
    }
}
