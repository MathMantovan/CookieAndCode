using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Producao;

/// <summary>
/// Value Object representing the unit yield of a Lote (DDD: VO — immutable, no identity, structural equality).
/// Must be positive: a zero yield would cause division by zero when computing CostPerUnit.
/// Uses the static factory method pattern to enforce the invariant before construction.
/// </summary>
public sealed class Rendimento : ValueObject
{
    /// <summary>The number of units produced in this batch.</summary>
    public int Value { get; }

    private Rendimento(int value) => Value = value;

    /// <summary>
    /// Creates a <see cref="Rendimento"/> with the given value.
    /// Throws <see cref="DomainException"/> when <paramref name="value"/> is zero or negative.
    /// </summary>
    public static Rendimento Create(int value)
    {
        if (value <= 0) throw new DomainException("Yield must be positive.");
        return new Rendimento(value);
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
