using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class EntradaMercaderiaConfiguration : IEntityTypeConfiguration<EntradaMercaderia>
{
    public void Configure(EntityTypeBuilder<EntradaMercaderia> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("entradas_mercaderia");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ProveedorId).HasColumnName("proveedor_id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.FechaEntrada).HasColumnName("fecha_entrada").HasColumnType("date");
        builder.Property(x => x.NroDocumento).HasColumnName("nro_documento").HasMaxLength(50);
        builder.Property(x => x.FechaRegistro).HasColumnName("fecha_registro").HasColumnType("datetime");
        builder.HasIndex(x => x.ProveedorId);
        builder.HasIndex(x => x.UsuarioId);
        builder.HasOne(x => x.Proveedor).WithMany(x => x.Entradas).HasForeignKey(x => x.ProveedorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Usuario).WithMany(x => x.Entradas).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
    }
}
