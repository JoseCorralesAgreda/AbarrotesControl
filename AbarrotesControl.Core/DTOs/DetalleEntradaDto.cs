namespace AbarrotesControl.Core.DTOs;

public class DetalleEntradaDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public int StockActual { get; set; }
}
