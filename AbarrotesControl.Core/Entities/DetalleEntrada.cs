namespace AbarrotesControl.Core.Entities;

public partial class DetalleEntrada
{
    public int Id { get; set; }
    public int EntradaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public virtual EntradaMercaderia Entrada { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}
