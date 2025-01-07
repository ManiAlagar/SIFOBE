using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class TherapeuticPlanValidator : AbstractValidator<TherapeuticPlanRequest>
    {
        public TherapeuticPlanValidator()
        {
            RuleFor(x => x.DateFrom)
                .NotEmpty().WithMessage("date from is required.");
            RuleFor(x => x.DateTo)
               .NotEmpty().WithMessage("date to is required.");

            RuleForEach(x => x.therapeuticPlanDetailRequests)
               .ChildRules(contact =>
               {
                   //contact.RuleFor(c => c.AIC).NotEmpty().WithMessage("");
               });
        }
    }
}
