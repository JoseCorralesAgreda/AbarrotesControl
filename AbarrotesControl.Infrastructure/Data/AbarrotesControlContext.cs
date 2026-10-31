using System.Reflection;
using AbarrotesControl.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Data;

public partial class AbarrotesControlContext : DbContext
{
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<Proveedor> Proveedores { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<EntradaMercaderia> EntradasMercaderia { get; set; }
    public virtual DbSet<DetalleEntrada> DetallesEntrada { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }
    public virtual DbSet<DetalleVenta> DetallesVenta { get; set; }
    public virtual DbSet<AjusteStock> AjustesStock { get; set; }
    public virtual DbSet<MovimientoStock> MovimientosStock { get; set; }

    public AbarrotesControlContext(DbContextOptions<AbarrotesControlContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
