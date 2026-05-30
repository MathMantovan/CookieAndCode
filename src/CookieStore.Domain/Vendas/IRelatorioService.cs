namespace CookieStore.Domain.Vendas;

/// <summary>
/// Abstraction for the cross-BC reporting domain service (DDD: Domain Service interface — allows
/// Application layer to depend on an abstraction rather than the concrete implementation, honoring DIP).
/// </summary>
public interface IRelatorioService
{
    /// <summary>Returns profit per flavor for all flavors that had sales or production in the given period.</summary>
    Task<IEnumerable<LucroPorSabor>> GetProfitBySaborAsync(DateTime from, DateTime to);

    /// <summary>Returns an aggregated sales summary for the given period.</summary>
    Task<ResumoVendas> GetSummaryAsync(DateTime from, DateTime to);
}
