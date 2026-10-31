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

[ApiController]
[Route("api/productos")]
public class ProductosController(IProductoService service, IMapper mapper,
    IValidator<CrearProductoDto> validator,
    IValidator<ProductoQueryFilter> filtroValidator, IValidator<int> idValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductoQueryFilter filtro)
    {
        var validation = await filtroValidator.ValidateAsync(filtro);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<List<ProductoDto>>(mapper.Map<List<ProductoDto>>(await service.GetAllAsync(filtro))));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var validation = await idValidator.ValidateAsync(id);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<ProductoDto>(mapper.Map<ProductoDto>(await service.GetByIdAsync(id))));
    }

    [HttpPost]
    public async Task<IActionResult> Post(CrearProductoDto dto)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));

        var entity = await service.InsertAsync(mapper.Map<Producto>(dto));
        return CreatedAtAction(nameof(GetById), new { id = entity.Id },
            new ApiResponse<ProductoDto>(mapper.Map<ProductoDto>(entity)));
    }
}
