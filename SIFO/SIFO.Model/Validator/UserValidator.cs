using FluentValidation;
using Microsoft.AspNetCore.Http;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        private readonly IHttpContextAccessor _httpcontextAccessor;
        public UserValidator(IHttpContextAccessor _contextAccessor)
        {
            _httpcontextAccessor = _contextAccessor;
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("first name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("email is required.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("phone number is required.");
            RuleFor(x => x.PhoneNumber).MaximumLength(10).WithMessage("Phone number should not exceeds 10 characters .");
            RuleFor(x => x.AuthenticationType).NotEmpty().WithMessage("AuthenticationType is required.");
            RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country Id is required.");
            RuleFor(x => x.FiscalCode).MinimumLength(1).WithMessage("Fiscal code should not be empty .");
            RuleFor(x => x.FiscalCode).MaximumLength(16).WithMessage("Fiscal code should not exceeds 16 characters .");
            When(x => x.PharmacyIds != null, () =>
            {
                RuleFor(x => x.PharmacyIds).NotEmpty().WithMessage("Pharmacy ID list is required.")
                .ForEach(id => id.NotNull().WithMessage("Each Pharmacy ID is required."));
           
            });
            When(x => x.HospitalIds != null, () =>
            {
                RuleFor(x => x.PharmacyIds).NotEmpty().WithMessage("Hospital Id list is required.")
                .ForEach(id => id.NotNull().WithMessage("Each Hospital Id  is  required."));

            });
            When(x => _contextAccessor.HttpContext.Request.Method == "POST", () =>
            {
                RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("password is required.");
            });
            }
    }
}
