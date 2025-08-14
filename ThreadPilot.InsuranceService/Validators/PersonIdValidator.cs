using FluentValidation;

namespace ThreadPilot.InsuranceService.Validators;

public class PersonIdValidator : AbstractValidator<string>
{
    public PersonIdValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("Person ID is required")
            .Length(11)
            .WithMessage("Person ID must be exactly 11 characters")
            .Matches("^[0-9]+$")
            .WithMessage("Person ID can only contain numbers");
    }
}