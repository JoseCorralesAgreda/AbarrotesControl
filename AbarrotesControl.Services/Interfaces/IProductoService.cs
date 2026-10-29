using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<Producto>> GetAllAsync();
    Task<Producto> GetByIdAsync(int id);
    Task<Producto> InsertAsync(Producto producto);
}
