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
[Route("api/entradas")]
public class EntradasController(IEntradaService service, IMapper mapper,
    IValidator<CrearEntradaDto> validator) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) =>
        Ok(new ApiResponse<EntradaDto>(mapper.Map<EntradaDto>(await service.GetByIdAsync(id))));

    [HttpPost]
    public async Task<IActionResult> Post(CrearEntradaDto dto)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));

        var entity = await service.InsertAsync(mapper.Map<EntradaMercaderia>(dto));
        return CreatedAtAction(nameof(GetById), new { id = entity.Id },
            new ApiResponse<EntradaDto>(mapper.Map<EntradaDto>(entity)));
    }
}
