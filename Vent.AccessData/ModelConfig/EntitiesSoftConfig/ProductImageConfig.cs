using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class ProductImageConfig : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(e => e.ProductImageId);
        //Borrado En Cascada
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Product).WithMany(c => c.ProductImages).OnDelete(DeleteBehavior.Restrict);
    }
}