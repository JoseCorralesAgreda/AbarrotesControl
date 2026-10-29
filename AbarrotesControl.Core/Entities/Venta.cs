namespace AbarrotesControl.Core.Entities;

public partial class Venta : BaseEntity
{
    public int UsuarioId { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Total { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
