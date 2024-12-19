using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class HospitalValidator : AbstractValidator<HospitalRequest>
    {
        public HospitalValidator()
        {
            RuleFor(x => x.HospitalName)
                 .NotEmpty().WithMessage("hospital name is required.")
                 .MaximumLength(100).WithMessage("name cannot exceed 100 characters.");
            //RuleFor(x => x.CountryCode)
            //    .MaximumLength(3).WithMessage("COUNTRY_CODE code cannot exceed 2 characters.");
        }
    }
}
