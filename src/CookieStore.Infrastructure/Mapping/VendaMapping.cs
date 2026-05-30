using CookieStore.Domain.SharedKernel;
using CookieStore.Domain.Vendas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookieStore.Infrastructure.Mapping;

public class VendaMapping : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        // Id is now a plain Guid inherited from Entity — no conversion required
        builder.HasKey(v => v.Id);

        builder.Property(v => v.SaborId)
               .HasConversion(id => id.Value, value => new SaborId(value))
               .IsRequired();

        builder.OwnsOne(v => v.Quantity, qty =>
        {
            qty.Property(q => q.Value)
               .HasColumnName("Quantity")
               .IsRequired();
        });

        builder.Property(v => v.UnitPrice)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(v => v.SoldAt)
               .IsRequired();

        // Computed in memory — never persisted
        builder.Ignore(v => v.Total);

        // VendaId is a computed wrapper over Id — never persisted separately
        builder.Ignore(v => v.VendaId);
    }
}
