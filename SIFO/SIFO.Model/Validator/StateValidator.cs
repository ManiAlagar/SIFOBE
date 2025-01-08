using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class StateValidator : AbstractValidator<StateRequest>
    {
        public StateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("country name is required.")
                .MaximumLength(100).WithMessage("name cannot exceed 100 characters.");
            RuleFor(x => x.CountryCode)
             .MaximumLength(3).WithMessage("country code code cannot exceed 2 characters.");
        }
    }
}
