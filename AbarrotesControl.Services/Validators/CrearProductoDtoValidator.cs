using AbarrotesControl.Core.DTOs;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class CrearProductoDtoValidator : AbstractValidator<CrearProductoDto>
{
    public CrearProductoDtoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre debe tener como máximo 120 caracteres.");
        RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("La categoría debe ser mayor que cero.");
        RuleFor(x => x.PrecioVenta).GreaterThan(0).WithMessage("El precio de venta debe ser mayor que cero (RN-02).");
        RuleFor(x => x.PrecioVenta).PrecisionScale(10, 2, true)
            .WithMessage("El precio de venta admite hasta 8 dígitos enteros y 2 decimales.");
        RuleFor(x => x.StockMinimo).GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo (RN-03).");
        RuleFor(x => x.FechaVencimiento)
            .Must(fecha => !fecha.HasValue || fecha.Value >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de vencimiento no puede ser anterior a hoy (RN-04).");
    }
}
