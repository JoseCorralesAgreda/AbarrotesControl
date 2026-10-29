namespace AbarrotesControl.Core.Entities;

public partial class MovimientoStock : BaseEntity
{
    public int ProductoId { get; set; }
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int StockResultante { get; set; }
    public int? ReferenciaId { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public virtual Producto Producto { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
