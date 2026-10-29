using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("detalle_venta");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.VentaId).HasColumnName("venta_id");
        builder.Property(x => x.ProductoId).HasColumnName("producto_id");
        builder.Property(x => x.Cantidad).HasColumnName("cantidad").HasColumnType("int");
        builder.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(10, 2);
        builder.Property(x => x.Subtotal).HasColumnName("subtotal").HasPrecision(12, 2);
        builder.HasIndex(x => new { x.VentaId, x.ProductoId }).IsUnique();
        builder.HasIndex(x => x.VentaId);
        builder.HasOne(x => x.Venta).WithMany(x => x.Detalles).HasForeignKey(x => x.VentaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.ProductoId);
        builder.HasOne(x => x.Producto).WithMany(x => x.DetallesVenta).HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
    }
}
