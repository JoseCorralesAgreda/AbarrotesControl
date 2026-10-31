using AbarrotesControl.Core.QueryFilters;
using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class AlertaQueryFilterValidator : AbstractValidator<AlertaQueryFilter>
{
    public AlertaQueryFilterValidator()
    {
        RuleFor(x => x.Tipo)
            .Must(tipo => tipo is null or "SIN_STOCK" or "STOCK_BAJO" or "POR_VENCER" or "VENCIDO")
            .WithMessage("Tipo inválido. Use SIN_STOCK, STOCK_BAJO, POR_VENCER o VENCIDO.");
    }
}
