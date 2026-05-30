namespace CookieStore.Domain;

/// <summary>Base exception for all domain invariant violations (DDD: domain errors are explicit, never silent).</summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
