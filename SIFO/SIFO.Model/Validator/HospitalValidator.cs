using FluentValidation;
using SIFO.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Validator
{
    public class HospitalValidator : AbstractValidator<HospitalRequest>
    {
        public HospitalValidator()
        {
            RuleFor(x => x.HospitalName)
                 .NotEmpty().WithMessage("Country Name is required.")
                 .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            //RuleFor(x => x.CountryCode)
            //    .MaximumLength(3).WithMessage("COUNTRY_CODE code cannot exceed 2 characters.");
        }
    }
}
