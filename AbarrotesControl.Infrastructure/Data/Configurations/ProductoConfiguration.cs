using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("productos");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120);
        builder.Property(x => x.CategoriaId).HasColumnName("categoria_id");
        builder.Property(x => x.PrecioVenta).HasColumnName("precio_venta").HasColumnType("decimal(10,2)");
        builder.Property(x => x.StockMinimo).HasColumnName("stock_minimo");
        builder.Property(x => x.StockActual).HasColumnName("stock_actual");
        builder.Property(x => x.FechaVencimiento).HasColumnName("fecha_vencimiento").HasColumnType("date");
        builder.Property(x => x.Activo).HasColumnName("activo").HasColumnType("tinyint(1)");
        builder.Property(x => x.CreadoPor).HasColumnName("creado_por");
        builder.Property(x => x.FechaRegistro).HasColumnName("fecha_registro").HasColumnType("datetime");
        builder.HasIndex(x => x.Nombre).IsUnique();
        builder.HasIndex(x => x.CategoriaId);
        builder.HasIndex(x => x.CreadoPor);
        builder.HasOne(x => x.Categoria).WithMany(x => x.Productos).HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Usuario).WithMany(x => x.Productos).HasForeignKey(x => x.CreadoPor).OnDelete(DeleteBehavior.Restrict);
    }
}
