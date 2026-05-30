namespace CookieStore.Application.Sabores;

/// <summary>DTO for Sabor read operations (Application boundary — uses primitives, never domain types).</summary>
public record SaborDto(Guid Id, string Name, decimal CatalogPrice, bool IsActive);
