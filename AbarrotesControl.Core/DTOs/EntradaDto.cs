namespace AbarrotesControl.Core.DTOs;

public class EntradaDto
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateOnly FechaEntrada { get; set; }
    public string? NroDocumento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public List<DetalleEntradaDto> Detalles { get; set; } = new List<DetalleEntradaDto>();
}
