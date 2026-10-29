using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class AjusteStockConfiguration : IEntityTypeConfiguration<AjusteStock>
{
    public void Configure(EntityTypeBuilder<AjusteStock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("ajustes_stock");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProductoId).HasColumnName("producto_id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(20);
        builder.Property(x => x.Cantidad).HasColumnName("cantidad").HasColumnType("int");
        builder.Property(x => x.Motivo).HasColumnName("motivo").HasMaxLength(255);
        builder.Property(x => x.FechaAjuste).HasColumnName("fecha_ajuste").HasColumnType("datetime");
        builder.HasIndex(x => x.ProductoId);
        builder.HasOne(x => x.Producto).WithMany(x => x.AjustesStock).HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.Usuario).WithMany(x => x.AjustesStock).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
