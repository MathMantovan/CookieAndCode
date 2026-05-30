using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Producao;

/// <summary>Repository interface for the Lote aggregate (DDD: interface in Domain, implementation in Infrastructure).</summary>
public interface ILoteRepository
{
    Task<IEnumerable<Lote>> GetByPeriodAsync(DateTime from, DateTime to);
    Task<IEnumerable<Lote>> GetBySaborAsync(SaborId saborId);
    Task AddAsync(Lote lote);
}
