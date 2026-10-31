using FluentValidation;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController(IStockService service,
    IValidator<AlertaQueryFilter> filtroValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AlertaQueryFilter filtro)
    {
        var validation = await filtroValidator.ValidateAsync(filtro);
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));
        return Ok(new ApiResponse<ResumenAlertasDto>(await service.GetAlertasAsync(filtro)));
    }
}
