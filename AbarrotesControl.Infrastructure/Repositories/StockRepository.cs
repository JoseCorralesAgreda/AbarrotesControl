using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class StockRepository(AbarrotesControlContext context) : IStockRepository
{
    public Task<List<Producto>> GetAllAsync(int? categoriaId, string? buscar)
    {
        var query = context.Productos.AsNoTracking().Include(x => x.Categoria).Where(x => x.Activo);
        if (categoriaId.HasValue) query = query.Where(x => x.CategoriaId == categoriaId.Value);
        if (!string.IsNullOrWhiteSpace(buscar)) query = query.Where(x => x.Nombre.Contains(buscar.Trim()));
        return query.ToListAsync();
    }

    public Task<Producto?> GetByIdAsync(int productoId) => context.Productos.AsNoTracking()
        .Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == productoId && x.Activo);

    public Task<bool> ExisteCategoriaAsync(int categoriaId) =>
        context.Categorias.AsNoTracking().AnyAsync(x => x.Id == categoriaId);
}
