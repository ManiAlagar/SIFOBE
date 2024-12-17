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

            When(x => _contextAccessor.HttpContext.Request.Method == "POST", () =>
            {
                RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("password is required.");
            });
        }
    }
}
