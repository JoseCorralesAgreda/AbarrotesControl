using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController(ICategoriaService categorias, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new ApiResponse<List<CategoriaDto>>(mapper.Map<List<CategoriaDto>>(await categorias.GetAllAsync())));
}
