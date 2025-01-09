using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class CalendarValidator : AbstractValidator<CalendarRequest>
    {
        public CalendarValidator()
        {
            RuleFor(x => x.PharmacyId).NotNull().WithMessage(Constants.PHARMACY_ID_REQUIRED);
            RuleFor(x => x.CalendarDate).GreaterThan(DateTime.UtcNow.Date).WithMessage(Constants.CALENDAR_DATE_GREATER_THAN_TODAY);
        }
    }
}
