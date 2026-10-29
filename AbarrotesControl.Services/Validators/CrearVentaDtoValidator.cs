using AbarrotesControl.Core.DTOs;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class CrearVentaDtoValidator : AbstractValidator<CrearVentaDto>
{
    public CrearVentaDtoValidator()
    {
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("Debe incluir al menos un producto (RN-12).");
        RuleFor(x => x.Detalles)
            .Must(detalles => detalles is null || detalles.Where(x => x is not null)
                .Select(x => x.ProductoId).Distinct().Count() == detalles.Count)
            .WithMessage("No se permiten productos repetidos ni detalles nulos (RN-12).");
        RuleForEach(x => x.Detalles).SetValidator(new CrearDetalleVentaDtoValidator());
    }
}
