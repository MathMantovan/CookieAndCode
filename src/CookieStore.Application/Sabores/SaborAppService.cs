using CookieStore.Domain;
using CookieStore.Domain.SharedKernel;

namespace CookieStore.Application.Sabores;

/// <summary>Application Service for Sabor use cases (DDD: orchestrates only — no business logic).</summary>
public class SaborAppService
{
    private readonly ISaborRepository _repository;

    public SaborAppService(ISaborRepository repository) => _repository = repository;

    public async Task<SaborId> CreateAsync(string name, decimal catalogPrice)
    {
        var sabor = new Sabor(name, PrecoTabela.Create(catalogPrice));
        await _repository.AddAsync(sabor);
        return sabor.SaborId;
    }

    public async Task UpdatePriceAsync(SaborId id, decimal newPrice)
    {
        var sabor = await GetOrThrowAsync(id);
        sabor.UpdatePrice(PrecoTabela.Create(newPrice));
        await _repository.UpdateAsync(sabor);
    }

    public async Task DeactivateAsync(SaborId id)
    {
        var sabor = await GetOrThrowAsync(id);
        sabor.Deactivate();
        await _repository.UpdateAsync(sabor);
    }

    public async Task<IEnumerable<SaborDto>> GetAllActiveAsync()
    {
        var sabores = await _repository.GetAllActiveAsync();
        return sabores.Select(s => new SaborDto(s.SaborId.Value, s.Name, s.CatalogPrice.Value, s.IsActive));
    }

    private async Task<Sabor> GetOrThrowAsync(SaborId id)
    {
        var sabor = await _repository.GetByIdAsync(id);
        if (sabor is null) throw new DomainException("Sabor not found.");
        return sabor;
    }
}
