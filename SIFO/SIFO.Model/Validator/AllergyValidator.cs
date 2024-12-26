using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class AllergyValidator : AbstractValidator<AllergyRequest>
    {
        public AllergyValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("name is required.");
            RuleFor(x => x.IsActive.ToString())
               .NotEmpty().WithMessage("is active is required.");
        }
    }
}
