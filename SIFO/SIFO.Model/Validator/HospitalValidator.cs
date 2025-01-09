using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class HospitalValidator : AbstractValidator<HospitalRequest>
    {
        public HospitalValidator()
        {
            RuleFor(x => x.HospitalName)
                 .NotEmpty().WithMessage(Constants.HOSPITAL_NAME_REQUIRED)
                 .MaximumLength(100).WithMessage(Constants.NAME_TOO_LONG);
        }
    }
}
