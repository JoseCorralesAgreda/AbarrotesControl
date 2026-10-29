namespace AbarrotesControl.Core.Entities;

public partial class AjusteStock : BaseEntity
{
    public int ProductoId { get; set; }
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaAjuste { get; set; }
    public virtual Producto Producto { get; set; } = null!;
    public virtual Usuario Usuario { get; set; } = null!;
}
