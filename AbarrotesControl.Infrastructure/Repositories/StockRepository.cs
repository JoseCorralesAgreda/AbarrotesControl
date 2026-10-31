using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class StockRepository(AbarrotesControlContext context) : IStockRepository
{
    public Task<List<Producto>> GetAllAsync(StockQueryFilter filtro, DateOnly hoy, int diasVencimiento)
    {
        var query = context.Productos.AsNoTracking().Include(x => x.Categoria).Where(x => x.Activo);
        if (filtro.CategoriaId.HasValue) query = query.Where(x => x.CategoriaId == filtro.CategoriaId.Value);
        if (!string.IsNullOrWhiteSpace(filtro.Buscar)) query = query.Where(x => x.Nombre.Contains(filtro.Buscar.Trim()));
        if (filtro.SoloAlertas) query = FiltrarAlertas(query, null, hoy, diasVencimiento);
        return query.ToListAsync();
    }

    public Task<List<Producto>> GetAlertasAsync(AlertaQueryFilter filtro, DateOnly hoy, int diasVencimiento)
    {
        var query = context.Productos.AsNoTracking().Include(x => x.Categoria).Where(x => x.Activo);
        if (filtro.Tipo is not null) query = FiltrarAlertas(query, filtro.Tipo, hoy, diasVencimiento);
        return query.ToListAsync();
    }

    private static IQueryable<Producto> FiltrarAlertas(IQueryable<Producto> query, string? tipo,
        DateOnly hoy, int diasVencimiento)
    {
        var limite = hoy.AddDays(Math.Min(diasVencimiento, DateOnly.MaxValue.DayNumber - hoy.DayNumber));
        return tipo switch
        {
            "SIN_STOCK" => query.Where(x => x.StockActual == 0),
            "STOCK_BAJO" => query.Where(x => x.StockActual > 0 && x.StockActual <= x.StockMinimo),
            "POR_VENCER" => query.Where(x => x.FechaVencimiento >= hoy && x.FechaVencimiento <= limite),
            "VENCIDO" => query.Where(x => x.FechaVencimiento < hoy),
            _ => query.Where(x => x.StockActual == 0 ||
                (x.StockActual > 0 && x.StockActual <= x.StockMinimo) || x.FechaVencimiento <= limite)
        };
    }

    public Task<Producto?> GetByIdAsync(int productoId) => context.Productos.AsNoTracking()
        .Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == productoId && x.Activo);

    public Task<bool> ExisteCategoriaAsync(int categoriaId) =>
        context.Categorias.AsNoTracking().AnyAsync(x => x.Id == categoriaId);
}
