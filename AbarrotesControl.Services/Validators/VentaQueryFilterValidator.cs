using AbarrotesControl.Core.QueryFilters;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class VentaQueryFilterValidator : AbstractValidator<VentaQueryFilter>
{
    public VentaQueryFilterValidator()
    {
        RuleFor(x => x.Desde)
            .Must((filtro, desde) => !desde.HasValue || !filtro.Hasta.HasValue || desde.Value <= filtro.Hasta.Value)
            .WithMessage("Desde debe ser anterior o igual a Hasta.");
    }
}
