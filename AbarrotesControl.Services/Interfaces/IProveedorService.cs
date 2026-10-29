using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IProveedorService
{
    Task<IEnumerable<Proveedor>> GetAllAsync();
}
