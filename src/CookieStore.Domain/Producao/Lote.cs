using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Producao;

/// <summary>
/// Aggregate Root for the Producao BC (Supporting Domain).
/// CostPerUnit is computed from TotalCost and Yield — never persisted (EF Core must Ignore it).
/// Constructor is internal: only LoteFactory may instantiate this aggregate.
/// Inherits identity-based equality from <see cref="Entity"/>.
/// </summary>
public class Lote : Entity
{
    public SaborId SaborId { get; private set; }
    public Rendimento Yield { get; private set; }
    public CustoTotal TotalCost { get; private set; }
    public DateTime ProducedAt { get; private set; }

    /// <summary>Computed cost per unit (TotalCost / Yield). Never persisted.</summary>
    public decimal CostPerUnit => TotalCost.Value / Yield.Value;

    /// <summary>
    /// Strongly-typed identity wrapper over the base <see cref="Entity.Id"/> (DDD: prevents primitive obsession).
    /// Computed on demand — never persisted separately.
    /// </summary>
    public LoteId LoteId => new(Id);

    internal Lote(Guid id, SaborId saborId, Rendimento yield, CustoTotal totalCost, DateTime producedAt)
        : base(id)
    {
        SaborId = saborId;
        Yield = yield;
        TotalCost = totalCost;
        ProducedAt = producedAt;
    }

    // Required by EF Core — properties are populated via reflection before use
    private Lote() : base(Guid.Empty) { SaborId = null!; Yield = null!; TotalCost = null!; }

    /// <inheritdoc />
    public override string Describe() => $"Lote [SaborId={SaborId}] ProducedAt={ProducedAt:yyyy-MM-dd}";
}
