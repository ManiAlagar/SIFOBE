using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class PharmacyValidator : AbstractValidator<PharmacyRequest>
    {
        public PharmacyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("pharmacy name is required.");
            RuleFor(x => x.Province)
               .NotEmpty().WithMessage("hospital name is required.")
               .MaximumLength(2).WithMessage("province cannot exceed more than 2 characters.");
            RuleFor(x => x.CAP)
                .MaximumLength(6).WithMessage("cap code cannot exceed more than 6 characters.");
            RuleFor(x => x.PharmacyTypeId)
                .NotEmpty().WithMessage("pharmacy type is required.");
            RuleForEach(x => x.Contact)
                .ChildRules(contact =>
                {
                    contact.RuleFor(c => c.ContactName).NotEmpty().WithMessage("First name is required.");
                    contact.RuleFor(c => c.ContactSurname).NotEmpty().WithMessage("Last name is required.");
                    contact.RuleFor(c => c.Role).NotEmpty().WithMessage("role is required.");
                    contact.RuleFor(c => c.PhoneNumber).NotEmpty().WithMessage("phone number is required.")
                    .MaximumLength(15).WithMessage("phone number cannot exceed more than 15 characters.");
                });
        }
    }
}
