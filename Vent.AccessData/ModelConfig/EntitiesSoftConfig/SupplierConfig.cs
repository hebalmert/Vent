using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class SupplierConfig : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(e => e.SupplierId);
        builder.HasIndex(e => new { e.CorporationId, e.Name }).IsUnique();
        builder.HasIndex(e => new { e.CorporationId, e.NroDocument }).IsUnique();
        //Evitar el borrado en cascada
        builder.HasOne(e => e.State).WithMany(c => c.Suppliers).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.City).WithMany(c => c.Suppliers).OnDelete(DeleteBehavior.Restrict);
    }
}