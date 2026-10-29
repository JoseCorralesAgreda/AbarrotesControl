using AbarrotesControl.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AbarrotesControl.Api.Filters;

public class ModelStateValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
            throw new BusinessException("Error de validación", details: context.ModelState
                .SelectMany(entry => entry.Value!.Errors.Select(_ => new
                {
                    field = entry.Key,
                    error = "El valor enviado no es válido para este campo."
                })));
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
