using Microsoft.Extensions.Configuration;
using SendGrid;
using SIFO.Core.Service.Contracts;
using SIFO.Model.Response;
using System.Net.Mail;

namespace SIFO.Core.Service.Implementations
{
    public class SendGridService : ISendGridService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        public SendGridService(IConfiguration configuration,ISendGridClient sendGridClient)
        { 
            _sendGridClient = sendGridClient;
            _configuration = configuration;
        }
        public async Task<ApiResponse<string>> SendMailAsync(string toMail, string subject, string body, string name)
        {
            //var fromEmail = new EmailAddress(_configuration["SendGridSettings:From"], "SIFO");
            //var toEmail = new EmailAddress(toMail, name);
            //var htmlContent = body;
            //var mailMsg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, null, htmlContent);
            //try
            //{
            //    var response = await _sendGridClient.SendEmailAsync(mailMsg);
            //    if (response.IsSuccessStatusCode)
            //        return ApiResponse<string>.Success(Constants.SUCCESS);

            //    return ApiResponse<string>.InternalServerError();
            //}
            //catch (Exception ex)
            //{
            //    return ApiResponse<string>.InternalServerError(ex.Message);
            //} 
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_configuration["MailCredentials:Mail"]);
            mail.To.Add(new MailAddress(toMail));
            mail.Body = body;
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential(_configuration["MailCredentials:Mail"], _configuration["MailCredentials:Password"]);
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.Send(mail);
                return ApiResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.InternalServerError(ex.Message);
            }
        }
    }
}
