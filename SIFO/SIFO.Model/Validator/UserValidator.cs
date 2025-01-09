using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Constant;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using System.Security.Cryptography;
using System.Text;

namespace SIFO.Model.Validator
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        private readonly IHttpContextAccessor _httpcontextAccessor;
        public UserValidator(IHttpContextAccessor _contextAccessor)
        {
            _httpcontextAccessor = _contextAccessor;
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(Constants.LAST_NAME_REQUIRED);
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(Constants.FIRST_NAME_REQUIRED);
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Constants.EMAIL_REQUIRED);
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(Constants.PHONE_NUMBER_REQUIRED);
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(10).WithMessage(Constants.USER_PHONE_NUMBER_TOO_LONG);
            RuleFor(x => x.AuthenticationType)
                .NotEmpty().WithMessage(Constants.AUTHENTICATION_TYPE_REQUIRED);
            RuleFor(x => x.CountryCode)
                .NotEmpty().WithMessage(Constants.COUNTRY_CODE_REQUIRED);
            RuleFor(x => x.FiscalCode)
                .MinimumLength(1).WithMessage(Constants.FISCAL_CODE_REQUIRED);
            RuleFor(x => x.FiscalCode)
                .MaximumLength(16).WithMessage(Constants.FISCAL_CODE_TOO_LONG);
            When(x => x.PharmacyIds != null, () =>
            {
                RuleFor(x => x.PharmacyIds)
                    .NotEmpty().WithMessage(Constants.PHARMACY_ID_LIST_REQUIRED)
                    .ForEach(id => id.NotNull().WithMessage(Constants.EACH_PHARMACY_ID_REQUIRED));
           
            });
            When(x => x.HospitalIds != null, () =>
            {
                RuleFor(x => x.HospitalIds)
                    .NotEmpty().WithMessage(Constants.HOSPITAL_ID_LIST_REQUIRED)
                    .ForEach(id => id.NotNull().WithMessage(Constants.EACH_HOSPITAL_ID_REQUIRED));

            });
            When(x => _contextAccessor.HttpContext.Request.Method == "POST", () =>
            {
                RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("password is required.");
            }); 
        }
    }
}
