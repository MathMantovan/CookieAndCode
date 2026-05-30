using CookieStore.Domain.Producao;
using CookieStore.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookieStore.Infrastructure.Mapping;

public class LoteMapping : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> builder)
    {
        builder.ToTable("Lotes");

        // Id is now a plain Guid inherited from Entity — no conversion required
        builder.HasKey(l => l.Id);

        builder.Property(l => l.SaborId)
               .HasConversion(id => id.Value, value => new SaborId(value))
               .IsRequired();

        builder.OwnsOne(l => l.Yield, yield =>
        {
            yield.Property(y => y.Value)
                 .HasColumnName("Yield")
                 .IsRequired();
        });

        builder.OwnsOne(l => l.TotalCost, cost =>
        {
            cost.Property(c => c.Value)
                .HasColumnName("TotalCost")
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.Property(l => l.ProducedAt)
               .IsRequired();

        // Computed in memory — never persisted
        builder.Ignore(l => l.CostPerUnit);

        // LoteId is a computed wrapper over Id — never persisted separately
        builder.Ignore(l => l.LoteId);
    }
}
