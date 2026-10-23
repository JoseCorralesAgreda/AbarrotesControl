namespace AbarrotesControl.Core.DTOs;

public class CrearEntradaDto
{
    public int ProveedorId { get; set; }
    public DateOnly FechaEntrada { get; set; }
    public string? NroDocumento { get; set; }
    public List<CrearDetalleEntradaDto> Detalles { get; set; } = new List<CrearDetalleEntradaDto>();
}
