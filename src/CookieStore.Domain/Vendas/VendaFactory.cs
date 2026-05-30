namespace CookieStore.Domain.Vendas;

/// <summary>
/// Factory for Venda aggregates (DDD: Factory encapsulates creation complexity and enforces initial invariants).
/// Accepts a <see cref="SaborSnapshot"/> from the ACL boundary — the factory never touches the Sabor aggregate
/// directly, reinforcing the separation between the Vendas BC and the SharedKernel.
/// The snapshot price becomes the immutable UnitPrice of the new Venda.
/// </summary>
public class VendaFactory
{
    /// <summary>
    /// Creates a new <see cref="Venda"/> from the given ACL snapshot and quantity.
    /// Throws <see cref="DomainException"/> when <paramref name="quantity"/> is zero or negative.
    /// </summary>
    public Venda Create(SaborSnapshot snapshot, int quantity)
    {
        var qty = Quantidade.Create(quantity);

        return new Venda(
            Guid.NewGuid(),
            snapshot.Id,
            qty,
            snapshot.CurrentPrice,
            DateTime.UtcNow
        );
    }
}
