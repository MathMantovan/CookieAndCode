using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Producao;

/// <summary>
/// Value Object representing the total production cost of a Lote (DDD: VO — immutable, no identity, structural equality).
/// Uses the static factory method pattern to enforce the positive-value invariant before construction.
/// </summary>
public sealed class CustoTotal : ValueObject
{
    /// <summary>The monetary total cost for this production batch.</summary>
    public decimal Value { get; }

    private CustoTotal(decimal value) => Value = value;

    /// <summary>
    /// Creates a <see cref="CustoTotal"/> with the given value.
    /// Throws <see cref="DomainException"/> when <paramref name="value"/> is zero or negative.
    /// </summary>
    public static CustoTotal Create(decimal value)
    {
        if (value <= 0) throw new DomainException("Total cost must be positive.");
        return new CustoTotal(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
