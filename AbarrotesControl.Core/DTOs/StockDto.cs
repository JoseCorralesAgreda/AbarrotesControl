namespace AbarrotesControl.Core.DTOs;

public class StockDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public int? DiasParaVencer { get; set; }
    public List<string> Alertas { get; set; } = [];
}
