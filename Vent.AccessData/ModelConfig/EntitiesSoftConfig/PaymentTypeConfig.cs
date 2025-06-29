using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class PaymentTypeConfig : IEntityTypeConfiguration<PaymentType>
{
    public void Configure(EntityTypeBuilder<PaymentType> builder)
    {
        builder.HasKey(e => e.PaymentTypeId);
        builder.HasIndex(e => new { e.CorporationId, e.PaymentName }).IsUnique();
    }
}