using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IProveedorRepository
{
    Task<List<Proveedor>> GetAllAsync();
    Task<Proveedor?> GetByIdAsync(int id);
}
