namespace AbarrotesControl.Core.DTOs;

public class VentaDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Total { get; set; }
    public List<DetalleVentaDto> Detalles { get; set; } = [];
    public List<AlertaStockDto> Alertas { get; set; } = [];
}
