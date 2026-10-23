using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbarrotesControl.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("usuarios");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(100);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
        builder.Property(x => x.Rol).HasColumnName("rol").HasMaxLength(20);
        builder.Property(x => x.Activo).HasColumnName("activo").HasColumnType("tinyint(1)");
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
