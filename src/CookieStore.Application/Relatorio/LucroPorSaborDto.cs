namespace CookieStore.Application.Relatorio;

/// <summary>DTO for profit-per-flavor report output (Application boundary — mirrors LucroPorSabor domain result type).</summary>
public record LucroPorSaborDto(Guid SaborId, decimal Revenue, decimal Cost, decimal Margin);
