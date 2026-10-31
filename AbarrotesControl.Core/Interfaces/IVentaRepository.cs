using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IVentaRepository : IBaseRepository<Venta>
{
    Task<Venta?> GetByIdAsync(int id);
    Task<List<Venta>> GetAllAsync(VentaQueryFilter filtro);
    Task<Venta> InsertAsync(Venta venta);
}
