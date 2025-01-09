using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class HospitalFacilityValidator : AbstractValidator<HospitalFacilityRequest>
    {
        public HospitalFacilityValidator()
        {
            RuleFor(x => x.HospitalFacilityName)
                .NotEmpty().WithMessage(Constants.HOSPITAL_FACILITY_NAME_REQUIRED);
            RuleFor(x => x.Province)
               .NotEmpty().WithMessage(Constants.PROVINCE_REQUIRED) 
               .MaximumLength(2).WithMessage(Constants.PROVINCE_TOO_LONG);
            RuleFor(x => x.Address)
               .NotEmpty().WithMessage(Constants.ADDRESS_REQUIRED);
            RuleFor(x => x.City)
               .NotEmpty().WithMessage(Constants.CITY_REQUIRED);
            RuleFor(x => x.CAP)
                .NotEmpty().WithMessage(Constants.CAP_CODE_REQUIRED);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(Constants.PHONE_NUMBER_REQUIRED);
            RuleForEach(x => x.Contact)
                .ChildRules(contact =>
                {
                    contact.RuleFor(c => c.ContactName).NotEmpty().WithMessage(Constants.FIRST_NAME_REQUIRED);
                    contact.RuleFor(c => c.ContactSurname).NotEmpty().WithMessage(Constants.LAST_NAME_REQUIRED);
                    contact.RuleFor(c => c.Role).NotEmpty().WithMessage(Constants.ROLE_REQUIRED);
                    contact.RuleFor(c => c.PhoneNumber).NotEmpty().WithMessage(Constants.PHONE_NUMBER_REQUIRED)
                    .MaximumLength(15).WithMessage(Constants.PHONE_NUMBER_TOO_LONG);
                });
        }
    }
}
