using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class DocumentTypeConfig : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.HasKey(e => e.DocumentTypeId);
        builder.HasIndex(e => new { e.CorporationId, e.DocumentName }).IsUnique();
        builder.HasIndex(e => new { e.CorporationId, e.Abreviatura }).IsUnique();
    }
}