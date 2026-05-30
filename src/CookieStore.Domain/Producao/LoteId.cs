namespace CookieStore.Domain.Producao;

/// <summary>Strongly-typed identity for the Lote aggregate (DDD: prevents primitive obsession on Guid).</summary>
public record LoteId(Guid Value);
