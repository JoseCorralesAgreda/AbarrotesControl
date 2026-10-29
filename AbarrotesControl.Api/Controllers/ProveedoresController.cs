using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/proveedores")]
public class ProveedoresController(IProveedorService proveedores, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new ApiResponse<List<ProveedorDto>>(mapper.Map<List<ProveedorDto>>(await proveedores.GetAllAsync())));
}
