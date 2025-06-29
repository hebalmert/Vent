using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class SellConfig : IEntityTypeConfiguration<Sell>
{
    public void Configure(EntityTypeBuilder<Sell> builder)
    {
        builder.HasKey(e => e.SellId);
        builder.HasIndex(e => new { e.CorporationId, e.NroSell }).IsUnique();
        builder.Property(e => e.SellDate).HasColumnType("date");
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Usuario).WithMany(c => c.Sells).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Client).WithMany(c => c.Sells).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.PaymentType).WithMany(c => c.Sells).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ProductStorage).WithMany(c => c.Sells).OnDelete(DeleteBehavior.Restrict);
    }
}