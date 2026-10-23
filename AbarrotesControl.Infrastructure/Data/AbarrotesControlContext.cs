using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Data;

public class AbarrotesControlContext : DbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<EntradaMercaderia> EntradasMercaderia => Set<EntradaMercaderia>();
    public DbSet<DetalleEntrada> DetallesEntrada => Set<DetalleEntrada>();

    public AbarrotesControlContext(DbContextOptions<AbarrotesControlContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AbarrotesControlContext).Assembly);
    }
}
