using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>
/// Value Object representing the quantity of a sale (DDD: VO — immutable, no identity, structural equality).
/// Uses the static factory method pattern to enforce the positive-value invariant before construction.
/// </summary>
public sealed class Quantidade : ValueObject
{
    /// <summary>The integer quantity for this sale line.</summary>
    public int Value { get; }

    private Quantidade(int value) => Value = value;

    /// <summary>
    /// Creates a <see cref="Quantidade"/> with the given value.
    /// Throws <see cref="DomainException"/> when <paramref name="value"/> is zero or negative.
    /// </summary>
    public static Quantidade Create(int value)
    {
        if (value <= 0) throw new DomainException("Quantity must be positive.");
        return new Quantidade(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
