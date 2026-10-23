using AbarrotesControl.Core.DTOs;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class CrearDetalleEntradaDtoValidator : AbstractValidator<CrearDetalleEntradaDto>
{
    public CrearDetalleEntradaDtoValidator()
    {
        RuleFor(x => x.ProductoId).GreaterThan(0).WithMessage("El producto debe ser mayor que cero.");
        RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser un entero mayor que cero (RN-06).");
        RuleFor(x => x.CostoUnitario).GreaterThan(0).WithMessage("El costo unitario debe ser mayor que cero (RN-07).");
        RuleFor(x => x.CostoUnitario).PrecisionScale(10, 2, true)
            .WithMessage("El costo unitario admite hasta 8 dígitos enteros y 2 decimales.");
    }
}
