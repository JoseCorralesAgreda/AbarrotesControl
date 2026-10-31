using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class ProductoRepository(AbarrotesControlContext context) : BaseRepository<Producto>(context), IProductoRepository
{
    public Task<List<Producto>> GetAllAsync(ProductoQueryFilter filtro)
    {
        var query = context.Productos.AsNoTracking().Include(x => x.Categoria).AsQueryable();
        if (filtro.Nombre is not null) query = query.Where(x => x.Nombre.Contains(filtro.Nombre));
        if (filtro.CategoriaId.HasValue) query = query.Where(x => x.CategoriaId == filtro.CategoriaId.Value);
        if (filtro.Activo.HasValue) query = query.Where(x => x.Activo == filtro.Activo.Value);
        return query.ToListAsync();
    }

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
