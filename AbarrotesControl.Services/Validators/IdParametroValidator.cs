using FluentValidation;

namespace AbarrotesControl.Services.Validators;

public class IdParametroValidator : AbstractValidator<int>
{
    public IdParametroValidator()
    {
        RuleFor(id => id).GreaterThan(0).OverridePropertyName("Id")
            .WithMessage("El id debe ser mayor que cero.");
    }
}
