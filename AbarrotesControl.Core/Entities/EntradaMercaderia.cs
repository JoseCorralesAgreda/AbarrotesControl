namespace AbarrotesControl.Core.Entities;

public partial class EntradaMercaderia
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public int UsuarioId { get; set; }
    public DateOnly FechaEntrada { get; set; }
    public string? NroDocumento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public virtual Proveedor Proveedor { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<DetalleEntrada> Detalles { get; set; } = new List<DetalleEntrada>();
}
