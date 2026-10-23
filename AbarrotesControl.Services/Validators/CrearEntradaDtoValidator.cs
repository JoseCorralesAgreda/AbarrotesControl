using AbarrotesControl.Core.DTOs;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class CrearEntradaDtoValidator : AbstractValidator<CrearEntradaDto>
{
    public CrearEntradaDtoValidator()
    {
        RuleFor(x => x.ProveedorId).GreaterThan(0).WithMessage("El proveedor debe ser mayor que cero.");
        RuleFor(x => x.FechaEntrada)
            .GreaterThanOrEqualTo(new DateOnly(1000, 1, 1))
            .WithMessage("La fecha de entrada es obligatoria y debe ser válida para MySQL.")
            .Must(fecha => fecha <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de entrada no puede ser futura (RN-11).");
        RuleFor(x => x.NroDocumento).MaximumLength(50)
            .WithMessage("El número de documento debe tener como máximo 50 caracteres.");
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("Debe incluir al menos un detalle (RN-06).");
        RuleFor(x => x.Detalles)
            .Must(detalles => detalles is null || detalles.Where(x => x is not null)
                .Select(x => x.ProductoId).Distinct().Count() == detalles.Count)
            .WithMessage("No se permiten productos repetidos ni detalles nulos (RN-06).");
        RuleForEach(x => x.Detalles).SetValidator(new CrearDetalleEntradaDtoValidator());
    }
}
