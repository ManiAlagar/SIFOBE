using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class AllergyValidator : AbstractValidator<AllergyRequest>
    {
        public AllergyValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage(Constants.NAME_REQUIRED);
            RuleFor(x => x.IsActive.ToString())
               .NotEmpty().WithMessage(Constants.IS_ACTIVE_REQUIRED);
        }
    }
}
