namespace AbarrotesControl.Core.Entities;

public partial class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public virtual ICollection<EntradaMercaderia> Entradas { get; set; } = new List<EntradaMercaderia>();
}
