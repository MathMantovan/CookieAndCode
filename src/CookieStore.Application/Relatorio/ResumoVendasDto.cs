namespace CookieStore.Application.Relatorio;

/// <summary>DTO for overall sales summary report output (Application boundary — mirrors ResumoVendas domain result type).</summary>
public record ResumoVendasDto(decimal TotalRevenue, decimal TotalCost, decimal GlobalMargin);
