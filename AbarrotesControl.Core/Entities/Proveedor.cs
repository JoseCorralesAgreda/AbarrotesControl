namespace AbarrotesControl.Core.Entities;

public partial class Proveedor : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public virtual ICollection<EntradaMercaderia> Entradas { get; set; } = new List<EntradaMercaderia>();
}
