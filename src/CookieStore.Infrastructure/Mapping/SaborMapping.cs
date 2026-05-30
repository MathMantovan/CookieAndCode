using CookieStore.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookieStore.Infrastructure.Mapping;

public class SaborMapping : IEntityTypeConfiguration<Sabor>
{
    public void Configure(EntityTypeBuilder<Sabor> builder)
    {
        builder.ToTable("Sabores");

        // Id is now a plain Guid inherited from Entity — no conversion required
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.OwnsOne(s => s.CatalogPrice, price =>
        {
            price.Property(p => p.Value)
                 .HasColumnName("CatalogPrice")
                 .HasPrecision(18, 2)
                 .IsRequired();
        });

        builder.Property(s => s.IsActive)
               .IsRequired();

        // SaborId is a computed wrapper over Id — never persisted separately
        builder.Ignore(s => s.SaborId);
    }
}
