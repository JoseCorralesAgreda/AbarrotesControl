using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/stock")]
public class StockController(IStockService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoriaId, [FromQuery] string? buscar,
        [FromQuery] bool soloAlertas = false) =>
        Ok(new ApiResponse<List<StockDto>>(await service.GetAllAsync(categoriaId, buscar, soloAlertas)));

    [HttpGet("{productoId:int}")]
    public async Task<IActionResult> GetById(int productoId) =>
        Ok(new ApiResponse<StockDto>(await service.GetByIdAsync(productoId)));
}
