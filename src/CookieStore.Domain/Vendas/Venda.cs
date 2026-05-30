using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>
/// Aggregate Root for the Vendas BC (Core Domain).
/// UnitPrice is a snapshot of the catalog price at sale time — intentionally decoupled from Sabor (DDD: snapshot pattern).
/// Total is computed and never persisted — EF Core configuration must Ignore it.
/// Constructor is internal: only VendaFactory may instantiate this aggregate.
/// Inherits identity-based equality from <see cref="Entity"/>.
/// </summary>
public class Venda : Entity
{
    public SaborId SaborId { get; private set; }
    public Quantidade Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime SoldAt { get; private set; }

    /// <summary>Computed total for this sale line (UnitPrice × Quantity). Never persisted.</summary>
    public decimal Total => UnitPrice * Quantity.Value;

    /// <summary>
    /// Strongly-typed identity wrapper over the base <see cref="Entity.Id"/> (DDD: prevents primitive obsession).
    /// Computed on demand — never persisted separately.
    /// </summary>
    public VendaId VendaId => new(Id);

    internal Venda(Guid id, SaborId saborId, Quantidade quantity, decimal unitPrice, DateTime soldAt)
        : base(id)
    {
        SaborId = saborId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        SoldAt = soldAt;
    }

    // Required by EF Core — properties are populated via reflection before use
    private Venda() : base(Guid.Empty) { SaborId = null!; Quantity = null!; }

    /// <inheritdoc />
    public override string Describe() => $"Venda [SaborId={SaborId}] SoldAt={SoldAt:yyyy-MM-dd}";
}
