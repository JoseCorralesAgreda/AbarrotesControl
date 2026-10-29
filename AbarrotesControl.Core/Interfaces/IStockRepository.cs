using AbarrotesControl.Core.Entities;

namespace AbarrotesControl.Core.Interfaces;

public interface IStockRepository
{
    Task<List<Producto>> GetAllAsync(int? categoriaId, string? buscar);
    Task<Producto?> GetByIdAsync(int productoId);
    Task<bool> ExisteCategoriaAsync(int categoriaId);
}
