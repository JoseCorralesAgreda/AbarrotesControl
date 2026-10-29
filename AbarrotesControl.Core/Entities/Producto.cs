namespace AbarrotesControl.Core.Entities;

public partial class Producto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockMinimo { get; set; }
    public int StockActual { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public bool Activo { get; set; }
    public int CreadoPor { get; set; }
    public DateTime FechaRegistro { get; set; }
    public virtual Categoria Categoria { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<DetalleEntrada> DetallesEntrada { get; set; } = new List<DetalleEntrada>();
    public virtual ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    public virtual ICollection<AjusteStock> AjustesStock { get; set; } = new List<AjusteStock>();
    public virtual ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
}
