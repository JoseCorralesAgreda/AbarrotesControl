using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/entradas")]
public class EntradasController(
    IEntradaRepository entradas,
    IProveedorRepository proveedores,
    IProductoRepository productos,
    IValidator<CrearEntradaDto> validator,
    IMapper mapper,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entrada = await entradas.GetByIdAsync(id);
        if (entrada is null)
            return NotFound(new { message = $"No existe la entrada con id {id}." });
        return Ok(new ApiResponse<EntradaDto>(mapper.Map<EntradaDto>(entrada)));
    }

    [HttpPost]
    public async Task<IActionResult> Post(CrearEntradaDto dto)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(new
            {
                message = "Error de validación",
                errors = validation.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage })
            });

        var proveedor = await proveedores.GetByIdAsync(dto.ProveedorId);
        var proveedorInvalido = proveedor is null || !proveedor.Activo;
        var productosInvalidos = new List<int>();
        foreach (var detalle in dto.Detalles)
        {
            var producto = await productos.GetByIdAsync(detalle.ProductoId);
            if (producto is null || !producto.Activo)
                productosInvalidos.Add(detalle.ProductoId);
        }
        if (proveedorInvalido || productosInvalidos.Count > 0)
            return NotFound(new
            {
                message = "El proveedor o los productos indicados no existen o están inactivos (RN-08).",
                proveedorIds = proveedorInvalido ? new[] { dto.ProveedorId } : Array.Empty<int>(),
                productoIds = productosInvalidos
            });

        var entrada = mapper.Map<EntradaMercaderia>(dto);
        entrada.UsuarioId = configuration.GetValue<int>("UsuarioSemillaId");
        entrada.FechaRegistro = DateTime.Now;
        entrada = await entradas.InsertAsync(entrada);
        return CreatedAtAction(nameof(GetById), new { id = entrada.Id },
            new ApiResponse<EntradaDto>(mapper.Map<EntradaDto>(entrada)));
    }
}
