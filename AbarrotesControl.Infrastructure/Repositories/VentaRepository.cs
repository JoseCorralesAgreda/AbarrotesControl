using System.Net;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class VentaRepository(AbarrotesControlContext context) : BaseRepository<Venta>(context), IVentaRepository
{
    public Task<Venta?> GetByIdAsync(int id) => context.Ventas.AsNoTracking()
        .Include(x => x.Detalles).ThenInclude(x => x.Producto).FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Venta>> GetAllAsync() => context.Ventas.AsNoTracking()
        .Include(x => x.Detalles).ThenInclude(x => x.Producto)
        .OrderByDescending(x => x.FechaVenta).ThenByDescending(x => x.Id).Take(50).ToListAsync();

    public async Task<Venta> InsertAsync(Venta venta)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            context.Ventas.Add(venta);
            await context.SaveChangesAsync();
            foreach (var detalle in venta.Detalles.OrderBy(x => x.ProductoId))
            {
                var actualizados = await context.Productos
                    .Where(x => x.Id == detalle.ProductoId && x.Activo && x.StockActual >= detalle.Cantidad)
                    .ExecuteUpdateAsync(update => update.SetProperty(
                        x => x.StockActual, x => x.StockActual - detalle.Cantidad));
                if (actualizados != 1)
                {
                    var producto = await context.Productos.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == detalle.ProductoId);
                    throw new BusinessException("Stock insuficiente", HttpStatusCode.Conflict, new[]
                    {
                        new { productoId = detalle.ProductoId, producto = producto?.Nombre,
                            motivo = "Stock insuficiente o producto inactivo", disponible = producto?.StockActual ?? 0,
                            solicitado = detalle.Cantidad }
                    });
                }

                var stockResultante = await context.Productos.Where(x => x.Id == detalle.ProductoId)
                    .Select(x => x.StockActual).SingleAsync();
                context.MovimientosStock.Add(new MovimientoStock
                {
                    ProductoId = detalle.ProductoId, UsuarioId = venta.UsuarioId, Tipo = "VENTA",
                    Cantidad = -detalle.Cantidad, StockResultante = stockResultante,
                    ReferenciaId = venta.Id, FechaMovimiento = venta.FechaVenta
                });
            }
            await context.SaveChangesAsync();
            var resultado = await GetByIdAsync(venta.Id)
                ?? throw new InvalidOperationException("No se pudo recuperar la venta.");
            await transaction.CommitAsync();
            return resultado;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
