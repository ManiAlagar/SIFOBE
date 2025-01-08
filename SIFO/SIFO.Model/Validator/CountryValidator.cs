using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class CountryValidator : AbstractValidator<CountryRequest>
    {
        public CountryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Country Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Iso3)
                .Length(3).WithMessage("ISO-3 code cannot exceed 3 characters.");
            RuleFor(x => x.Iso2)
                .Length(2).WithMessage("ISO-2 code cannot exceed 2 characters.");
            RuleFor(x => x.PhoneCode)
                .MaximumLength(255).WithMessage("Phone code cannot exceed 255 characters.");
        }
    }
}
