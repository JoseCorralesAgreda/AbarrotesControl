namespace AbarrotesControl.Core.Entities;

public partial class Usuario : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public virtual ICollection<EntradaMercaderia> Entradas { get; set; } = new List<EntradaMercaderia>();
    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public virtual ICollection<AjusteStock> AjustesStock { get; set; } = new List<AjusteStock>();
    public virtual ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
}
