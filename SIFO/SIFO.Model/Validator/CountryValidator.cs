using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class CountryValidator : AbstractValidator<CountryRequest>
    {
        public CountryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Constants.COUNTRY_NAME_REQUIRED)
                .MaximumLength(100).WithMessage(Constants.NAME_TOO_LONG);
            RuleFor(x => x.Iso3)
                .Length(3).WithMessage(Constants.ISO3_CODE_TOO_LONG);
            RuleFor(x => x.Iso2)
                .Length(2).WithMessage(Constants.ISO2_CODE_TOO_LONG);
            RuleFor(x => x.PhoneCode)
                .MaximumLength(255).WithMessage(Constants.PHONE_CODE_TOO_LONG);
        }
    }
}
