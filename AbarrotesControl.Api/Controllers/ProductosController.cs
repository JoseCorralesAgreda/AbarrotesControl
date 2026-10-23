using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController(
    IProductoRepository productos,
    ICategoriaRepository categorias,
    IValidator<CrearProductoDto> validator,
    IMapper mapper,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new ApiResponse<List<ProductoDto>>(mapper.Map<List<ProductoDto>>(await productos.GetAllAsync())));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var producto = await productos.GetByIdAsync(id);
        if (producto is null)
            return NotFound(new { message = $"No existe el producto con id {id}." });
        return Ok(new ApiResponse<ProductoDto>(mapper.Map<ProductoDto>(producto)));
    }

    [HttpPost]
    public async Task<IActionResult> Post(CrearProductoDto dto)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new
            {
                message = "Error de validación",
                errors = validation.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage })
            });

        if (await categorias.GetByIdAsync(dto.CategoriaId) is null)
            return NotFound(new { message = $"No existe la categoría con id {dto.CategoriaId}." });
        if (await productos.ExisteNombreAsync(dto.Nombre))
            return Conflict(new { message = "Ya existe un producto con ese nombre" });

        var producto = mapper.Map<Producto>(dto);
        producto.StockActual = 0;
        producto.Activo = true;
        producto.FechaRegistro = DateTime.Now;
        producto.CreadoPor = configuration.GetValue<int>("UsuarioSemillaId");
        try
        {
            producto = await productos.InsertAsync(producto);
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            return Conflict(new { message = "Ya existe un producto con ese nombre" });
        }

        return CreatedAtAction(nameof(GetById), new { id = producto.Id },
            new ApiResponse<ProductoDto>(mapper.Map<ProductoDto>(producto)));
    }
}
