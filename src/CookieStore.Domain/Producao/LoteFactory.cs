using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Producao;

/// <summary>
/// Factory for Lote aggregates (DDD: Factory enforces invariants before instantiation).
/// Receives SaborId directly — no price snapshot needed, unlike VendaFactory.
/// </summary>
public class LoteFactory
{
    public Lote Create(SaborId saborId, int yield, decimal totalCost)
    {
        var y = Rendimento.Create(yield);
        var c = CustoTotal.Create(totalCost);

        return new Lote(
            Guid.NewGuid(),
            saborId,
            y,
            c,
            DateTime.UtcNow
        );
    }
}
