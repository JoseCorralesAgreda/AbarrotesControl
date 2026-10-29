using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("ventas");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.FechaVenta).HasColumnName("fecha_venta").HasColumnType("datetime");
        builder.Property(x => x.Total).HasColumnName("total").HasPrecision(12, 2);
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.Usuario).WithMany(x => x.Ventas).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
