using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IProductoRepository : IBaseRepository<Producto>
{
    Task<List<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre);
    Task<Producto> InsertAsync(Producto producto);
}
