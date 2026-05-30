namespace CookieStore.Application.Producao;

/// <summary>DTO for Lote read operations (Application boundary — uses primitives, never domain types).</summary>
public record LoteDto(Guid Id, Guid SaborId, int Yield, decimal TotalCost, decimal CostPerUnit, DateTime ProducedAt);
