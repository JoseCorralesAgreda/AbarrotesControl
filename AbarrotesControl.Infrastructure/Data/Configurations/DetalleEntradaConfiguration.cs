using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class DetalleEntradaConfiguration : IEntityTypeConfiguration<DetalleEntrada>
{
    public void Configure(EntityTypeBuilder<DetalleEntrada> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("detalle_entrada");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.EntradaId).HasColumnName("entrada_id");
        builder.Property(x => x.ProductoId).HasColumnName("producto_id");
        builder.Property(x => x.Cantidad).HasColumnName("cantidad");
        builder.Property(x => x.CostoUnitario).HasColumnName("costo_unitario").HasColumnType("decimal(10,2)");
        builder.HasIndex(x => new { x.EntradaId, x.ProductoId }).IsUnique();
        builder.HasIndex(x => x.ProductoId);
        builder.HasOne(x => x.Entrada).WithMany(x => x.Detalles).HasForeignKey(x => x.EntradaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Producto).WithMany(x => x.DetallesEntrada).HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
    }
}
