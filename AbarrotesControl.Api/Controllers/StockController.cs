using FluentValidation;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/stock")]
public class StockController(IStockService service,
    IValidator<StockQueryFilter> filtroValidator, IValidator<int> idValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] StockQueryFilter filtro)
    {
        var validation = await filtroValidator.ValidateAsync(filtro);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<List<StockDto>>(await service.GetAllAsync(filtro)));
    }

    [HttpGet("{productoId:int}")]
    public async Task<IActionResult> GetById(int productoId)
    {
        var validation = await idValidator.ValidateAsync(productoId);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<StockDto>(await service.GetByIdAsync(productoId)));
    }
}
