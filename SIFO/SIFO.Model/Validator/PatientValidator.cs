using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class PatientValidator : AbstractValidator<PatientRequest>
    {
        public PatientValidator()
        {
        }
    }
}
