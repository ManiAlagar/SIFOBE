using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Entity;

namespace SIFO.Model.Validator
{
    public class OtpRequestValidator : AbstractValidator<OtpRequestModel>
    {
        private readonly SIFOContext _context;

        public OtpRequestValidator(SIFOContext context)
        {
            _context = context;
            RuleFor(x => x.OtpCode).NotEmpty().Length(6).WithMessage("OtpCode must be 6 characters.")
                .MustAsync(ValidateOtpCode).WithMessage("Invalid OTP Code.");
        }

        private async Task<bool> ValidateOtpCode(string otpCode, CancellationToken cancellationToken)
        {
            var otp = await _context.OtpRequests.Where(o => o.OtpCode == otpCode).FirstOrDefaultAsync(cancellationToken);
            return otp != null;
        }
    }

    public class OtpRequestModel
    {
        public string AuthenticationType { get; set; }
        public string AuthenticationFor { get; set; }
        public string OtpCode { get; set; }
        public string UserId { get; set; }
    }  
}
