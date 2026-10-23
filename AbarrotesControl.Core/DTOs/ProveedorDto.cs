namespace AbarrotesControl.Core.DTOs;

public class ProveedorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
}
