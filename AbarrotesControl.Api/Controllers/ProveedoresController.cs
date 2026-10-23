using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/proveedores")]
public class ProveedoresController(IProveedorRepository proveedores, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new ApiResponse<List<ProveedorDto>>(mapper.Map<List<ProveedorDto>>(await proveedores.GetAllAsync())));
}
