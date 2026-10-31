using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Services.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

// RN-17: los registros no admiten PUT ni DELETE.
[ApiController]
[Route("api/ventas")]
public class VentasController(IVentaService service, IMapper mapper,
    IValidator<CrearVentaDto> validator, IConfiguration configuration,
    IValidator<VentaQueryFilter> filtroValidator, IValidator<int> idValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] VentaQueryFilter filtro)
    {
        var validation = await filtroValidator.ValidateAsync(filtro);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<List<VentaDto>>(await service.GetAllAsync(filtro)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var validation = await idValidator.ValidateAsync(id);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<VentaDto>(await service.GetByIdAsync(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Post(CrearVentaDto dto)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        var venta = mapper.Map<Venta>(dto);
        venta.UsuarioId = int.Parse(configuration["UsuarioSemillaId"] ?? "0");
        venta.FechaVenta = DateTime.Now;
        var resultado = await service.InsertAsync(venta);
        return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, new ApiResponse<VentaDto>(resultado));
    }
}
