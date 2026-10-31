using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Services.Interfaces;

public interface IProductoService
{
    Task<IEnumerable<Producto>> GetAllAsync(ProductoQueryFilter filtro);
    Task<Producto> GetByIdAsync(int id);
    Task<Producto> InsertAsync(Producto producto);
}
