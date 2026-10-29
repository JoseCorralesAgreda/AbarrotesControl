using System.Net;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace AbarrotesControl.Services.Services;

public class ProductoService(IProductoRepository productos, IBaseRepository<Categoria> categorias,
    IConfiguration configuration) : IProductoService
{
    public async Task<IEnumerable<Producto>> GetAllAsync() => await productos.GetAllAsync();

    public async Task<Producto> GetByIdAsync(int id) => await productos.GetByIdAsync(id)
        ?? throw new BusinessException($"No existe el producto con id {id}.", HttpStatusCode.NotFound);

    public async Task<Producto> InsertAsync(Producto producto)
    {
        if (await categorias.GetById(producto.CategoriaId) is null)
            throw new BusinessException($"No existe la categoría con id {producto.CategoriaId}.", HttpStatusCode.NotFound);
        if (await productos.ExisteNombreAsync(producto.Nombre))
            throw new BusinessException("Ya existe un producto con ese nombre", HttpStatusCode.Conflict);

        producto.StockActual = 0;
        producto.Activo = true;
        producto.FechaRegistro = DateTime.Now;
        producto.CreadoPor = int.Parse(configuration["UsuarioSemillaId"] ?? "0");
        try
        {
            return await productos.InsertAsync(producto);
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new BusinessException("Ya existe un producto con ese nombre", HttpStatusCode.Conflict);
        }
    }
}
