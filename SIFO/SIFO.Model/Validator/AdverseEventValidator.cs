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
            RuleFor(x => x.PatientId)
             .NotEmpty().WithMessage("patient id is required.");
            RuleFor(x => x.Intensity)
               .NotEmpty().WithMessage("intensity is required.")
               .Must(name => name == "mild" || name == "average" || name == "serious")
               .WithMessage("intensity must be either 'mild' or 'average' or 'serious'.");
        }
    }
}
