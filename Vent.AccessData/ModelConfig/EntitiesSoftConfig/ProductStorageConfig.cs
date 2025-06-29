﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vent.Shared.EntitiesSoft;

namespace Vent.AccessData.ModelConfig.EntitiesSoftConfig;

public class ProductStorageConfig : IEntityTypeConfiguration<ProductStorage>
{
    public void Configure(EntityTypeBuilder<ProductStorage> builder)
    {
        builder.HasKey(e => e.ProductStorageId);
        builder.HasIndex(e => new { e.CorporationId, e.StorageName }).IsUnique();
        //Borrado En Cascada
        //Evitar el borrado en cascada
        builder.HasOne(e => e.State).WithMany(c => c.ProductStorages).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.City).WithMany(c => c.ProductStorages).OnDelete(DeleteBehavior.Restrict);
    }
}