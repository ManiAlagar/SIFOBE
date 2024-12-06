using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class CalendarValidator : AbstractValidator<CalendarRequest>
    {
        public CalendarValidator()
        {
            RuleFor(x => x.PharmacyId).NotNull().WithMessage("Pharmacy id is required");
            RuleFor(x => x.CalendarDate).GreaterThan(DateTime.UtcNow.Date).WithMessage("calendar date must be greater than today");
        }
    }
}
