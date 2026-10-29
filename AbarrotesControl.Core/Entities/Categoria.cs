namespace AbarrotesControl.Core.Entities;

public partial class Categoria : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
