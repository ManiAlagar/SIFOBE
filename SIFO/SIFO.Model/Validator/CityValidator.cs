using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class CityValidator : AbstractValidator<CityRequest>
    {
        public CityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Constants.COUNTRY_NAME_REQUIRED)
                .MaximumLength(100).WithMessage(Constants.NAME_TOO_LONG);
            RuleFor(x => x.CountryCode)
                .MaximumLength(3).WithMessage(Constants.COUNTRY_CODE_TOO_LONG);
            RuleFor(x => x.StateCode)
                .MaximumLength(3).WithMessage(Constants.STATE_CODE_TOO_LONG);
        }
    }
}
