using CookieStore.Application.Producao;
using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;

namespace CookieStore.Application.Producao;

/// <summary>Application Service for Lote use cases (DDD: orchestrates only — no business logic).</summary>
public class LoteAppService
{
    private readonly ILoteRepository _lotes;
    private readonly LoteFactory _factory;

    public LoteAppService(ILoteRepository lotes, LoteFactory factory)
    {
        _lotes = lotes;
        _factory = factory;
    }

    public async Task<LoteId> RegisterAsync(SaborId saborId, int yield, decimal totalCost)
    {
        var lote = _factory.Create(saborId, yield, totalCost);
        await _lotes.AddAsync(lote);
        return lote.LoteId;
    }

    public async Task<IEnumerable<LoteDto>> GetByPeriodAsync(DateTime from, DateTime to)
    {
        var lotes = await _lotes.GetByPeriodAsync(from, to);
        return lotes.Select(l => new LoteDto(
            l.LoteId.Value, l.SaborId.Value, l.Yield.Value, l.TotalCost.Value, l.CostPerUnit, l.ProducedAt));
    }
}
