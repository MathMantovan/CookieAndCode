namespace CookieStore.Domain.SharedKernel;

/// <summary>Repository interface for the Sabor aggregate (DDD: interface in Domain, implementation in Infrastructure).</summary>
public interface ISaborRepository
{
    Task<Sabor?> GetByIdAsync(SaborId id);
    Task<IEnumerable<Sabor>> GetAllActiveAsync();
    Task AddAsync(Sabor sabor);
    Task UpdateAsync(Sabor sabor);
}
