using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class PatientAnalysisReportValidator : AbstractValidator<PatientAnalysisReportRequest>
    {
        public PatientAnalysisReportValidator()
        {
            RuleFor(x => x.Attachment_Type).NotEmpty().WithMessage("attachment type is required.");
            RuleFor(x => x.File).NotEmpty().WithMessage("file path is required.");
            RuleFor(x => x.PatientId).NotEqual(0).NotEmpty().WithMessage("patientId is required.");
        }
    }
}
