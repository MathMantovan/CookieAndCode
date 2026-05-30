namespace CookieStore.Domain.SharedKernel;

/// <summary>
/// Abstract base class for all Domain Entities (DDD: Entity — identity-based equality).
/// Two entities are equal when they share the same runtime type and the same Id.
/// Subclasses receive their Id via constructor; the base never generates a new Guid by default,
/// keeping the identity-assignment responsibility with the Factory or the aggregate itself.
/// </summary>
public abstract class Entity
{
    /// <summary>Surrogate identity of this entity — a unique Guid assigned at creation time.</summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Initialises the entity with the given identity.
    /// Concrete subclasses call this from their own constructor.
    /// </summary>
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Returns a human-readable description of this entity.
    /// Subclasses should override to include domain-meaningful context.
    /// </summary>
    public virtual string Describe() => $"{GetType().Name} [Id={Id}]";

    /// <summary>Identity-based equality — two entities of the same type with the same Id are equal.</summary>
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        if (ReferenceEquals(this, obj)) return true;
        return Id == ((Entity)obj).Id;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>Equality operator delegating to <see cref="Equals"/>.</summary>
    public static bool operator ==(Entity? a, Entity? b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Equals(b);
    }

    /// <summary>Inequality operator delegating to <see cref="Equals"/>.</summary>
    public static bool operator !=(Entity? a, Entity? b) => !(a == b);
}
