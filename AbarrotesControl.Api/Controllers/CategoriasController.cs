using AbarrotesControl.Api.Response;
using AbarrotesControl.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AbarrotesControl.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController(ICategoriaRepository categorias) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resultado = (await categorias.GetAllAsync()).Select(x => new { x.Id, x.Nombre }).ToList();
        return Ok(new ApiResponse<object>(resultado));
    }
}
