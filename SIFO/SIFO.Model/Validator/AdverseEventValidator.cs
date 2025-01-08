using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class AdverseEventValidator : AbstractValidator<AdverseEventRequest>
    {
        public AdverseEventValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("name is required.");
        }
    }
}
