namespace CookieStore.Domain.SharedKernel;

/// <summary>
/// Value Object representing the catalog price of a Sabor (DDD: VO — immutable, no identity, structural equality).
/// Uses the static factory method pattern to enforce the positive-value invariant before construction.
/// </summary>
public sealed class PrecoTabela : ValueObject
{
    /// <summary>The monetary value of this catalog price.</summary>
    public decimal Value { get; }

    private PrecoTabela(decimal value) => Value = value;

    /// <summary>
    /// Creates a <see cref="PrecoTabela"/> with the given value.
    /// Throws <see cref="DomainException"/> when <paramref name="value"/> is zero or negative.
    /// </summary>
    public static PrecoTabela Create(decimal value)
    {
        if (value <= 0) throw new DomainException("Catalog price must be positive.");
        return new PrecoTabela(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
