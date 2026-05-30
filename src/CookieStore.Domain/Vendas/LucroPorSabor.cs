using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>Read model produced by RelatorioService for profit-per-flavor reporting (DDD: Domain Service result type).</summary>
public record LucroPorSabor(SaborId SaborId, decimal Revenue, decimal Cost, decimal Margin);
