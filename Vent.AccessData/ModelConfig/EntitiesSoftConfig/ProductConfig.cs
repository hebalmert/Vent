using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.ProductId);
        builder.HasIndex(e => new { e.CorporationId, e.ProductName }).IsUnique();
        //Borrado En Cascada
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Tax).WithMany(c => c.Products).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Category).WithMany(c => c.Products).OnDelete(DeleteBehavior.Restrict);
    }
}