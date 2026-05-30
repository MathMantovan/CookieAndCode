using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;

namespace CookieStore.Application.Vendas;

/// <summary>
/// Application Service for Venda use cases (DDD: orchestrates only — no business logic).
/// Depends on <see cref="ISaborAcl"/> (Anti-Corruption Layer) instead of <see cref="ISaborRepository"/>
/// directly, so that the Vendas BC is shielded from the full Sabor aggregate model.
/// </summary>
public class VendaAppService
{
    private readonly ISaborAcl _acl;
    private readonly IVendaRepository _vendas;
    private readonly VendaFactory _factory;

    public VendaAppService(ISaborAcl acl, IVendaRepository vendas, VendaFactory factory)
    {
        _acl = acl;
        _vendas = vendas;
        _factory = factory;
    }

    /// <summary>
    /// Registers a new sale for the given Sabor and quantity.
    /// Retrieves a <see cref="SaborSnapshot"/> via the ACL, captures the price, then persists the Venda.
    /// Throws <see cref="DomainException"/> when the Sabor does not exist or quantity is invalid.
    /// </summary>
    public async Task<VendaId> RegisterAsync(SaborId saborId, int quantity)
    {
        var snapshot = await _acl.GetSnapshotAsync(saborId)
            ?? throw new DomainException("Sabor not found.");

        var venda = _factory.Create(snapshot, quantity);
        await _vendas.AddAsync(venda);
        return venda.VendaId;
    }

    /// <summary>Returns all sales within the given period as DTOs.</summary>
    public async Task<IEnumerable<VendaDto>> GetByPeriodAsync(DateTime from, DateTime to)
    {
        var vendas = await _vendas.GetByPeriodAsync(from, to);
        return vendas.Select(v => new VendaDto(
            v.VendaId.Value, v.SaborId.Value, v.Quantity.Value, v.UnitPrice, v.Total, v.SoldAt));
    }
}
