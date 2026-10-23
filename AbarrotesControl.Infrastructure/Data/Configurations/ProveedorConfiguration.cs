using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("proveedores");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120);
        builder.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(20);
        builder.Property(x => x.Activo).HasColumnName("activo").HasColumnType("tinyint(1)");
    }
}
