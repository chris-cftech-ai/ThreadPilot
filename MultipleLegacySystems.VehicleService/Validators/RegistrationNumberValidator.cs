using FluentValidation;

namespace MultipleLegacySystems.VehicleService.Validators;

public class RegistrationNumberValidator : AbstractValidator<string>
{
    public RegistrationNumberValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("Registration number is required")
            .Length(3, 8)
            .WithMessage("Registration number must be between 3 and 8 characters")
            .Matches("^[A-Za-z0-9]+$")
            .WithMessage("Registration number can only contain letters and numbers");
    }
}