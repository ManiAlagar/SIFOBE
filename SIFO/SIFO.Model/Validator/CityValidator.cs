using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class CityValidator : AbstractValidator<CityRequest>
    {
        public CityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Country Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.CountryCode)
                .MaximumLength(3).WithMessage("COUNTRY_CODE code cannot exceed 2 characters.");
        }
    }
}
