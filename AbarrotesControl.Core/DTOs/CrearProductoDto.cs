namespace AbarrotesControl.Core.DTOs;

public class CrearProductoDto
{
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockMinimo { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
}
