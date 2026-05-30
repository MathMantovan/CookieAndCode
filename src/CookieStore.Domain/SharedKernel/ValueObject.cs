namespace CookieStore.Domain.SharedKernel;

/// <summary>
/// Abstract base class for all Value Objects (DDD: VO — structural equality, immutability, no identity).
/// Subclasses implement <see cref="GetEqualityComponents"/> to declare which fields participate in equality.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Returns all components that define the structural identity of this Value Object.
    /// Every field that contributes to equality must be yielded here.
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>Structural equality — two VOs are equal when all their components are equal.</summary>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        return GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());
    }

    /// <inheritdoc />
    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(c => c?.GetHashCode() ?? 0)
            .Aggregate((a, b) => a ^ b);

    /// <summary>Equality operator delegating to <see cref="Equals"/>.</summary>
    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Equals(b);
    }

    /// <summary>Inequality operator delegating to <see cref="Equals"/>.</summary>
    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
}
