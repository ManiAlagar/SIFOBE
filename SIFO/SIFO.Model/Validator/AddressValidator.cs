using FluentValidation;
using SIFO.Model.Constant;
using SIFO.Model.Request;

namespace SIFO.Model.Validator
{
    public class AddressValidator : AbstractValidator<AddressDetailRequest>
    {
        public AddressValidator() 
        {
            RuleFor(x => x.address)
               .NotEmpty().WithMessage(Constants.ADDRESS_NAME_REQUIRED);
        }
    }
}
