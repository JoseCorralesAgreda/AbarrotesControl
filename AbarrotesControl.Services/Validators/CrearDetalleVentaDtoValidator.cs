using AbarrotesControl.Core.DTOs;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class CrearDetalleVentaDtoValidator : AbstractValidator<CrearDetalleVentaDto>
{
    public CrearDetalleVentaDtoValidator()
    {
        RuleFor(x => x.ProductoId).GreaterThan(0).WithMessage("El producto debe ser mayor que cero.");
        RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser un entero mayor que cero (RN-12).");
    }
}
