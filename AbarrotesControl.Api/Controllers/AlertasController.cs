using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/alertas")]
public class AlertasController(IStockService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? tipo) =>
        Ok(new ApiResponse<ResumenAlertasDto>(await service.GetAlertasAsync(tipo)));
}
