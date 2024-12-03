using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("first name is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("email is required.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("phone number is required."); 
            RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("password is required."); 
        }
    }
}
