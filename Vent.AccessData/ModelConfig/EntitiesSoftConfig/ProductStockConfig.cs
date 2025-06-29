﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class ProductStockConfig : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.HasKey(e => e.ProductStockId);
        builder.HasIndex(e => new { e.CorporationId, e.ProductId, e.ProductStorageId }).IsUnique();
        //Borrado En Cascada
        //Evitar el borrado en cascada
        builder.HasOne(e => e.Product).WithMany(c => c.ProductStocks).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ProductStorage).WithMany(c => c.ProductStocks).OnDelete(DeleteBehavior.Restrict);
    }
}