using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IStockRepository
{
    Task<List<Producto>> GetAllAsync(StockQueryFilter filtro, DateOnly hoy, int diasVencimiento);
    Task<List<Producto>> GetAlertasAsync(AlertaQueryFilter filtro, DateOnly hoy, int diasVencimiento);
    Task<Producto?> GetByIdAsync(int productoId);
    Task<bool> ExisteCategoriaAsync(int categoriaId);
}
