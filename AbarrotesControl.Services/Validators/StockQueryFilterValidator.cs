using AbarrotesControl.Core.QueryFilters;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class StockQueryFilterValidator : AbstractValidator<StockQueryFilter>
{
    public StockQueryFilterValidator()
    {
        RuleFor(x => x.Buscar).MaximumLength(120)
            .WithMessage("La búsqueda debe tener como máximo 120 caracteres.");
        RuleFor(x => x.CategoriaId).GreaterThan(0).When(x => x.CategoriaId.HasValue)
            .WithMessage("La categoría debe ser mayor que cero.");
    }
}
