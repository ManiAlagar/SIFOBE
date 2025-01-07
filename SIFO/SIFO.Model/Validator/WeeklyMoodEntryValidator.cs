using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class WeeklyMoodEntryValidator : AbstractValidator<WeeklyMoodEntryRequest>
    {
        public WeeklyMoodEntryValidator()
        {
            //RuleFor(x => x.address)
            //   .NotEmpty().WithMessage("address name is required.");
        }
    }
}
