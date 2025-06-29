using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class TaxConfig : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.HasKey(e => e.TaxId);
        builder.HasIndex(e => new { e.CorporationId, e.TaxName }).IsUnique();
        builder.HasIndex(e => new { e.CorporationId, e.Rate }).IsUnique();
    }
}