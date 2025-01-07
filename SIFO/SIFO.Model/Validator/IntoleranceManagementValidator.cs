using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class IntoleranceManagementValidator : AbstractValidator<IntoleranceManagementRequest>
    {
        public IntoleranceManagementValidator()
        {
            RuleFor(x => x.Description)
               .NotEmpty().WithMessage("description is required.");
            RuleFor(x => x.IsActive.ToString())
               .NotEmpty().WithMessage("is active is required.");
            RuleFor(x => x.PatientId)
              .NotEmpty().WithMessage("patient id is required.");
        }
    }
}
