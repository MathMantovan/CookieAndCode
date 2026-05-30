using CookieStore.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CookieStore.Infrastructure.Repositories;

/// <summary>EF Core implementation of ISaborRepository (DDD: Repository implementation in Infrastructure).</summary>
public class SaborRepository : ISaborRepository
{
    private readonly CookieDbContext _context;

    public SaborRepository(CookieDbContext context) => _context = context;

    public async Task<Sabor?> GetByIdAsync(SaborId id) =>
        await _context.Sabores.FirstOrDefaultAsync(s => s.Id == id.Value);

    public async Task<IEnumerable<Sabor>> GetAllActiveAsync() =>
        await _context.Sabores
                      .AsNoTracking()
                      .Where(s => s.IsActive)
                      .ToListAsync();

    public async Task AddAsync(Sabor sabor)
    {
        await _context.Sabores.AddAsync(sabor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Sabor sabor)
    {
        _context.Sabores.Update(sabor);
        await _context.SaveChangesAsync();
    }
}
