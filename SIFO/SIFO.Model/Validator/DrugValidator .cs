using FluentValidation;
using SIFO.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIFO.Model.Validator
{
    public class DrugValidator : AbstractValidator<DrugRequest>
    {
        public DrugValidator()
        {
            RuleFor(x => x.AIC.Trim())
                .NotEmpty().WithMessage("AIC is required.");
            RuleFor(x => x.ExtendedDescription.Trim())
                .NotEmpty().WithMessage("ExtendedDescription is required.");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required.");
            RuleFor(x => x.CompanyName.Trim())
                .NotEmpty().WithMessage("CompanyName is required.");
            RuleFor(x => x.DrugRegionRequests)
                .NotEmpty().WithMessage("DrugRegionRequests are required.");
            When(x => x.DD || x.DPC, () =>
            {
                RuleFor(x => x.ProductType.Trim())
                    .NotEmpty().WithMessage("ProductType is required when DD or DPC is flagged.");
                RuleFor(x => x.Class.Trim())
                    .NotEmpty().WithMessage("Class is required when DD or DPC is flagged.");
                RuleFor(x => x.PharmaceuticalForm.Trim())
                    .NotEmpty().WithMessage("PharmaceuticalForm is required when DD or DPC is flagged.");
                RuleFor(x => x.UMR)
                    .NotEmpty().WithMessage("UMR is required when DD or DPC is flagged.")
                    .GreaterThan(0).WithMessage("UMR must be greater than zero.");
                RuleFor(x => x.PrescriptionType.Trim())
                    .NotEmpty().WithMessage("PrescriptionType is required when DD or DPC is flagged.");
                RuleFor(x => x.ProductImage.Trim())
                    .NotEmpty().WithMessage("ProductImage is required when DD or DPC is flagged.");
                RuleFor(x => x.TherapeuticIndications.Trim())
                    .NotEmpty().WithMessage("TherapeuticIndications are required when DD is flagged.");
                RuleFor(x => x.Temperature.Trim())
                    .NotEmpty().WithMessage("Temperature is required when DD or DPC is flagged.");
                RuleFor(x => x.DrugDosage.Trim())
                    .NotEmpty().WithMessage("DrugDosage is required when DD is flagged.");
                RuleFor(x => x.DrugRegionRequests)
                    .Must(drugRegionRequests => drugRegionRequests != null && drugRegionRequests.Any())
                    .WithMessage("At least one region is required when DD or DPC is true.");
                RuleForEach(x => x.DrugRegionRequests).Must(dr => dr.DrugType.ToUpper() == "DD" || dr.DrugType.ToUpper() == "DPC").
                WithMessage("Each DrugType in DrugRegionRequests must be either 'DD' or 'DPC'.");
            });
        }
    }
}
