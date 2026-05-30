namespace CookieStore.Domain.Vendas;

/// <summary>Read model produced by RelatorioService for overall sales summary (DDD: Domain Service result type).</summary>
public record ResumoVendas(decimal TotalRevenue, decimal TotalCost, decimal GlobalMargin);
