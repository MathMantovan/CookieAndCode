using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using Microsoft.EntityFrameworkCore;

namespace CookieStore.Infrastructure.Repositories;

/// <summary>EF Core implementation of IVendaRepository (DDD: Repository implementation in Infrastructure).</summary>
public class VendaRepository : IVendaRepository
{
    private readonly CookieDbContext _context;

    public VendaRepository(CookieDbContext context) => _context = context;

    public async Task<IEnumerable<Venda>> GetByPeriodAsync(DateTime from, DateTime to) =>
        await _context.Vendas
                      .AsNoTracking()
                      .Where(v => v.SoldAt >= from && v.SoldAt <= to)
                      .ToListAsync();

    public async Task<IEnumerable<Venda>> GetBySaborAsync(SaborId saborId) =>
        await _context.Vendas
                      .AsNoTracking()
                      .Where(v => v.SaborId == saborId)
                      .ToListAsync();

    public async Task AddAsync(Venda venda)
    {
        await _context.Vendas.AddAsync(venda);
        await _context.SaveChangesAsync();
    }
}
