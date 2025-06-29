using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class ClientConfig : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(e => e.ClientId);
        builder.HasIndex(e => new { e.CorporationId, e.FullName }).IsUnique();
        builder.HasIndex(e => new { e.CorporationId, e.NroDocument }).IsUnique();
        builder.HasIndex(e => e.UserName).IsUnique();
        //Evitar el borrado en cascada
        builder.HasOne(e => e.DocumentType).WithMany(c => c.Clients).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.State).WithMany(c => c.Clients).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.City).WithMany(c => c.Clients).OnDelete(DeleteBehavior.Restrict);
    }
}