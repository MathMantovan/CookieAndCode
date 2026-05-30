namespace CookieStore.Domain.SharedKernel;

/// <summary>
/// Aggregate Root for the SharedKernel BC.
/// Shared across Vendas and Producao BCs via the Shared Kernel pattern (DDD: Context Map).
/// CatalogPrice is always the current list price — snapshot happens in VendaFactory, not here.
/// Inherits identity-based equality from <see cref="Entity"/>.
/// </summary>
public class Sabor : Entity
{
    public string Name { get; private set; }
    public PrecoTabela CatalogPrice { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Strongly-typed identity wrapper over the base <see cref="Entity.Id"/> (DDD: prevents primitive obsession).
    /// Computed on demand — never persisted separately.
    /// </summary>
    public SaborId SaborId => new(Id);

    public Sabor(string name, PrecoTabela catalogPrice) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Sabor name must not be empty.");

        Name = name;
        CatalogPrice = catalogPrice;
        IsActive = true;
    }

    // Required by EF Core — properties are populated via reflection before use
    private Sabor() : base(Guid.Empty) { Name = null!; CatalogPrice = null!; }

    /// <summary>Updates the catalog price to <paramref name="newPrice"/>.</summary>
    public void UpdatePrice(PrecoTabela newPrice)
    {
        CatalogPrice = newPrice;
    }

    /// <summary>Marks this Sabor as inactive. Idempotent — calling on an already inactive Sabor is safe.</summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <inheritdoc />
    public override string Describe() => $"Sabor [{Name}] Id={Id}";
}
