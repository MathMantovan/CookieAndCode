namespace CookieStore.Application.Vendas;

/// <summary>DTO for Venda read operations (Application boundary — uses primitives, never domain types).</summary>
public record VendaDto(Guid Id, Guid SaborId, int Quantity, decimal UnitPrice, decimal Total, DateTime SoldAt);
