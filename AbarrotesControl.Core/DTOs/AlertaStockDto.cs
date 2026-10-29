namespace AbarrotesControl.Core.DTOs;

public class AlertaStockDto
{
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public int? DiasParaVencer { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
