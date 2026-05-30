using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;

namespace CookieStore.Application.Vendas;

/// <summary>
/// ACL adapter: translates <see cref="ISaborRepository"/> (SharedKernel) into <see cref="SaborSnapshot"/> (Vendas BC view).
/// Implements <see cref="ISaborAcl"/> — lives in the Application layer as the translation bridge
/// between the SharedKernel and the Vendas Bounded Context.
/// Callers in the Vendas BC receive only a snapshot; the full Sabor aggregate stays in SharedKernel.
/// </summary>
public class SaborAclAdapter : ISaborAcl
{
    private readonly ISaborRepository _repository;

    public SaborAclAdapter(ISaborRepository repository) => _repository = repository;

    /// <inheritdoc />
    public async Task<SaborSnapshot?> GetSnapshotAsync(SaborId id)
    {
        var sabor = await _repository.GetByIdAsync(id);
        return sabor is null
            ? null
            : new SaborSnapshot(sabor.SaborId, sabor.CatalogPrice.Value, sabor.IsActive);
    }
}
