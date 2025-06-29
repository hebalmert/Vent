using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class RegisterConfig : IEntityTypeConfiguration<Register>
{
    public void Configure(EntityTypeBuilder<Register> builder)
    {
        builder.HasKey(e => e.RegisterId);
        builder.HasIndex(e => new { e.CorporationId, e.RegPurchase }).IsUnique();
        builder.HasIndex(e => new { e.CorporationId, e.RegSells }).IsUnique();
    }
}