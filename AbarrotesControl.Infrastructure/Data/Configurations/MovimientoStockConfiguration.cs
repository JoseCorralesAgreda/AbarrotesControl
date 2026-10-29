using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class MovimientoStockConfiguration : IEntityTypeConfiguration<MovimientoStock>
{
    public void Configure(EntityTypeBuilder<MovimientoStock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("movimientos_stock");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProductoId).HasColumnName("producto_id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(20);
        builder.Property(x => x.Cantidad).HasColumnName("cantidad").HasColumnType("int");
        builder.Property(x => x.StockResultante).HasColumnName("stock_resultante");
        builder.Property(x => x.ReferenciaId).HasColumnName("referencia_id");
        builder.Property(x => x.FechaMovimiento).HasColumnName("fecha_movimiento").HasColumnType("datetime");
        builder.HasIndex(x => x.ProductoId);
        builder.HasOne(x => x.Producto).WithMany(x => x.MovimientosStock).HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.Usuario).WithMany(x => x.MovimientosStock).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
