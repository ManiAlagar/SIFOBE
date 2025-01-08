using FluentValidation;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class AddressValidator : AbstractValidator<AddressDetailRequest>
    {
        public AddressValidator() 
        {
            RuleFor(x => x.address)
               .NotEmpty().WithMessage("address name is required.");

        }
    }
}
