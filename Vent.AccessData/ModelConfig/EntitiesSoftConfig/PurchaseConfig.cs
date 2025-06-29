﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class PurchaseConfig : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(e => e.PurchaseId);
        builder.HasIndex(e => new { e.CorporationId, e.SupplierId, e.NroFactura }).IsUnique();
        builder.HasIndex(e => new { e.CorporationId, e.NroPurchase }).IsUnique();
        builder.Property(e => e.FacuraDate).HasColumnType("date");
        builder.Property(e => e.PurchaseDate).HasColumnType("date");
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Supplier).WithMany(c => c.Purchases).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ProductStorage).WithMany(c => c.Purchases).OnDelete(DeleteBehavior.Restrict);
    }
}