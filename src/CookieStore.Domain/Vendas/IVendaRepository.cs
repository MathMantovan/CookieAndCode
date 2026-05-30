using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>Repository interface for the Venda aggregate (DDD: interface in Domain, implementation in Infrastructure).</summary>
public interface IVendaRepository
{
    Task<IEnumerable<Venda>> GetByPeriodAsync(DateTime from, DateTime to);
    Task<IEnumerable<Venda>> GetBySaborAsync(SaborId saborId);
    Task AddAsync(Venda venda);
}
