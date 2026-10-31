using AbarrotesControl.Core.QueryFilters;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class ProductoQueryFilterValidator : AbstractValidator<ProductoQueryFilter>
{
    public ProductoQueryFilterValidator()
    {
        RuleFor(x => x.Nombre).MaximumLength(120)
            .WithMessage("El nombre debe tener como máximo 120 caracteres.");
        RuleFor(x => x.CategoriaId).GreaterThan(0).When(x => x.CategoriaId.HasValue)
            .WithMessage("La categoría debe ser mayor que cero.");
    }
}
