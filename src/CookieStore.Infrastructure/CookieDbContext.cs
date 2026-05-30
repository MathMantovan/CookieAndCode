using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using Microsoft.EntityFrameworkCore;

namespace CookieStore.Infrastructure;

/// <summary>
/// EF Core DbContext (DDD: Infrastructure concern — never referenced by Domain or Application layers).
/// All entity configuration is in IEntityTypeConfiguration classes, applied via ApplyConfigurationsFromAssembly.
/// </summary>
public class CookieDbContext : DbContext
{
    public DbSet<Sabor> Sabores { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<Lote> Lotes { get; set; }

    public CookieDbContext(DbContextOptions<CookieDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CookieDbContext).Assembly);
    }
}
