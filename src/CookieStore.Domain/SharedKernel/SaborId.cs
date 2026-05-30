namespace CookieStore.Domain.SharedKernel;

/// <summary>Strongly-typed identity for the Sabor aggregate (DDD: prevents primitive obsession on Guid).</summary>
public record SaborId(Guid Value);
