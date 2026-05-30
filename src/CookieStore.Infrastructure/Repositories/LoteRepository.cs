using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CookieStore.Infrastructure.Repositories;

/// <summary>EF Core implementation of ILoteRepository (DDD: Repository implementation in Infrastructure).</summary>
public class LoteRepository : ILoteRepository
{
    private readonly CookieDbContext _context;

    public LoteRepository(CookieDbContext context) => _context = context;

    public async Task<IEnumerable<Lote>> GetByPeriodAsync(DateTime from, DateTime to) =>
        await _context.Lotes
                      .AsNoTracking()
                      .Where(l => l.ProducedAt >= from && l.ProducedAt <= to)
                      .ToListAsync();

    public async Task<IEnumerable<Lote>> GetBySaborAsync(SaborId saborId) =>
        await _context.Lotes
                      .AsNoTracking()
                      .Where(l => l.SaborId == saborId)
                      .ToListAsync();

    public async Task AddAsync(Lote lote)
    {
        await _context.Lotes.AddAsync(lote);
        await _context.SaveChangesAsync();
    }
}
