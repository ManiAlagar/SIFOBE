using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class IntoleranceManagementValidator : AbstractValidator<IntoleranceManagementRequest>
    {
        public IntoleranceManagementValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("name is required.");
            RuleFor(x => x.IsActive.ToString())
               .NotEmpty().WithMessage("is active is required.");
        }
    }
}
