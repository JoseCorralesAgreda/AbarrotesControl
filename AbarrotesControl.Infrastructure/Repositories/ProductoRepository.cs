using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class ProductoRepository(AbarrotesControlContext context) : IProductoRepository
{
    public Task<List<Producto>> GetAllAsync() =>
        context.Productos.AsNoTracking().Include(x => x.Categoria).ToListAsync();

    public Task<Producto?> GetByIdAsync(int id) =>
        context.Productos.AsNoTracking().Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);

    public Task<bool> ExisteNombreAsync(string nombre) =>
        context.Productos.AnyAsync(x => x.Nombre == nombre);

    public async Task<Producto> InsertAsync(Producto producto)
    {
        context.Productos.Add(producto);
        await context.SaveChangesAsync();
        await context.Entry(producto).Reference(x => x.Categoria).LoadAsync();
        return producto;
    }
}
