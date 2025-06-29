using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class TransferDetailsConfig : IEntityTypeConfiguration<TransferDetails>
{
    public void Configure(EntityTypeBuilder<TransferDetails> builder)
    {
        builder.HasKey(e => e.TransferDetailsId);
        builder.HasIndex(e => new { e.CorporationId, e.ProductId, e.TransferId }).IsUnique();
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Product).WithMany(c => c.TransferDetails).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Transfer).WithMany(c => c.TransferDetails).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Category).WithMany(c => c.TransferDetails).OnDelete(DeleteBehavior.Restrict);
    }
}