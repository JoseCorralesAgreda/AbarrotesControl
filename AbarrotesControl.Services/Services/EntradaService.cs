using System.Net;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AbarrotesControl.Services.Services;

public class EntradaService(IEntradaRepository entradas, IBaseRepository<Proveedor> proveedores,
    IProductoRepository productos, IConfiguration configuration) : IEntradaService
{
    public async Task<EntradaMercaderia> GetByIdAsync(int id) => await entradas.GetByIdAsync(id)
        ?? throw new BusinessException($"No existe la entrada con id {id}.", HttpStatusCode.NotFound);

    public async Task<EntradaMercaderia> InsertAsync(EntradaMercaderia entrada)
    {
        var proveedor = await proveedores.GetById(entrada.ProveedorId);
        var proveedorInvalido = proveedor is null || !proveedor.Activo;
        var productosInvalidos = new List<int>();
        foreach (var detalle in entrada.Detalles)
        {
            var producto = await productos.GetByIdAsync(detalle.ProductoId);
            if (producto is null || !producto.Activo)
                productosInvalidos.Add(detalle.ProductoId);
        }
        if (proveedorInvalido || productosInvalidos.Count > 0)
            throw new BusinessException(
                "El proveedor o los productos indicados no existen o están inactivos (RN-08).",
                HttpStatusCode.NotFound, new
                {
                    proveedorIds = proveedorInvalido ? new[] { entrada.ProveedorId } : Array.Empty<int>(),
                    productoIds = productosInvalidos
                });

        entrada.UsuarioId = int.Parse(configuration["UsuarioSemillaId"] ?? "0");
        entrada.FechaRegistro = DateTime.Now;
        try
        {
            return await entradas.InsertAsync(entrada);
        }
        catch (InvalidOperationException ex) when (ex.Message is
            "El proveedor ya no está disponible." or "Un producto ya no está disponible.")
        {
            throw new BusinessException(ex.Message, HttpStatusCode.NotFound);
        }
    }
}
