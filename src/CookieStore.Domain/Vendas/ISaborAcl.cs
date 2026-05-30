using CookieStore.Domain.SharedKernel;

namespace CookieStore.Domain.Vendas;

/// <summary>
/// Anti-Corruption Layer interface: translation boundary between the SharedKernel and the Vendas BC.
/// Vendas BC depends only on this abstraction — never on the full Sabor aggregate.
/// Callers work with SaborSnapshot, which shields Vendas from changes in SharedKernel internals.
/// </summary>
public interface ISaborAcl
{
    /// <summary>
    /// Returns a <see cref="SaborSnapshot"/> for the given <paramref name="id"/>,
    /// or <c>null</c> when no Sabor with that id exists.
    /// </summary>
    Task<SaborSnapshot?> GetSnapshotAsync(SaborId id);
}
