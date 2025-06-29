using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class SellDetailsConfig : IEntityTypeConfiguration<SellDetails>
{
    public void Configure(EntityTypeBuilder<SellDetails> builder)
    {
        builder.HasKey(e => e.SellDetailsId);
        builder.HasIndex(e => new { e.CorporationId, e.ProductId, e.SellId }).IsUnique();
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Product).WithMany(c => c.SellDetails).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Sell).WithMany(c => c.SellDetails).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Category).WithMany(c => c.SellDetails).OnDelete(DeleteBehavior.Restrict);
    }
}