using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Infrastructure.Repositories;

public class EntradaRepository(AbarrotesControlContext context) : IEntradaRepository
{
    public Task<EntradaMercaderia?> GetByIdAsync(int id) =>
        context.EntradasMercaderia.AsNoTracking()
            .Include(x => x.Proveedor)
            .Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<EntradaMercaderia> InsertAsync(EntradaMercaderia entrada)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (!await context.Proveedores.AnyAsync(x => x.Id == entrada.ProveedorId && x.Activo))
                throw new InvalidOperationException("El proveedor ya no está disponible.");

            context.EntradasMercaderia.Add(entrada);
            await context.SaveChangesAsync();

            // Incrementos atómicos y orden estable para entradas concurrentes.
            foreach (var detalle in entrada.Detalles.OrderBy(x => x.ProductoId))
            {
                var actualizados = await context.Productos
                    .Where(x => x.Id == detalle.ProductoId && x.Activo)
                    .ExecuteUpdateAsync(update => update.SetProperty(
                        x => x.StockActual, x => x.StockActual + detalle.Cantidad));
                if (actualizados != 1)
                    throw new InvalidOperationException("Un producto ya no está disponible.");
            }

            var resultado = await GetByIdAsync(entrada.Id)
                ?? throw new InvalidOperationException("No se pudo recuperar la entrada.");
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
