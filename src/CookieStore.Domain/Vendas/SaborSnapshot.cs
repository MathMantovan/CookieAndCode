using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>
/// Read model of Sabor as seen by the Vendas BC (DDD: Anti-Corruption Layer).
/// Protects the Vendas BC from changes in the Sabor aggregate's internal model.
/// Immutable record — structurally equal when all properties match.
/// Constructor parameters use camelCase; public properties use PascalCase per C# convention.
/// </summary>
public record SaborSnapshot
{
    /// <summary>The strongly-typed identity of the Sabor at the time of the snapshot.</summary>
    public SaborId Id { get; init; }

    /// <summary>The catalog price of the Sabor at the time of the snapshot.</summary>
    public decimal CurrentPrice { get; init; }

    /// <summary>Whether the Sabor was active at the time this snapshot was captured.</summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Constructs a <see cref="SaborSnapshot"/>.
    /// All parameters use camelCase per C# constructor convention; properties use PascalCase.
    /// </summary>
    public SaborSnapshot(SaborId id, decimal currentPrice, bool isActive)
    {
        Id = id;
        CurrentPrice = currentPrice;
        IsActive = isActive;
    }
}
