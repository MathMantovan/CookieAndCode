namespace CookieStore.Domain.Vendas;

/// <summary>Strongly-typed identity for the Venda aggregate (DDD: prevents primitive obsession on Guid).</summary>
public record VendaId(Guid Value);
