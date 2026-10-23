namespace AbarrotesControl.Core.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public int StockMinimo { get; set; }
    public int StockActual { get; set; }
    public DateOnly? FechaVencimiento { get; set; }
    public bool Activo { get; set; }
}
