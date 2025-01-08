using Twilio;
using System.Text;
using Twilio.Types;
using System.Net.Mail;
using SIFO.Model.Entity;
using SIFO.Model.Constant;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Twilio.Rest.Api.V2010.Account;
using SIFO.Model.Request;
using SIFO.Core.Service.Contracts;


namespace SIFO.Utility.Implementations
{
    public class CommonService : ICommonService
    {
        private readonly IConfiguration _configuration;
        SIFOContext _context;
        IHttpContextAccessor _contextAccessor;
        ISendGridService _sendGridService;
        ITwilioService _twilioService;

        public CommonService(IConfiguration configuration, SIFOContext context, IHttpContextAccessor contextAccessor, ISendGridService sendGridService, ITwilioService twilioService)
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            _context = context;
            _contextAccessor = contextAccessor;
            _sendGridService = sendGridService;
            _twilioService = twilioService;
        }

        public async Task<string> GenerateOTP(long length)
        {
            if (length < 1)
                length = 6; 
            Random random = new Random();
            const string characters = "0123456789";

            var otpBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                otpBuilder.Append(characters[random.Next(characters.Length)]);
            }

            return otpBuilder.ToString();
        }

        public async Task<string> GenerateRandomPassword(long length = 12)
        {
            if (length < 1)
                length = 12;
            Random random = new Random();
            string password = string.Empty;
            string select = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            while (password.Length < length)
            {
                password = password + select[random.Next(select.Length - 1)];
            }
            return password;
        }

        public async Task<TokenResponse?> GetDataFromToken()
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("user not authenticated");
            }
            var claims = httpContext.User.Claims;
            var user = new TokenResponse
            {
                UserId = claims.FirstOrDefault(c => c.Type == "UserId")?.Value,
                Role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value, 
                RoleId = Convert.ToInt64(claims.FirstOrDefault(c => c.Type == "RoleId")?.Value),
                FullName = claims.FirstOrDefault(c => c.Type == "UserName")?.Value,
                Email = claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                ParentRoleId = claims.FirstOrDefault(c => c.Type == "ParentRoleId")?.Value

            };
            return user;
        }

        public async Task<string> EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            string pass = _configuration["PasswordEncryption:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(pass);
            var iv = Encoding.UTF8.GetBytes(pass);
            byte[] encryptedBytes;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = keyBytes;
                rijAlg.IV = iv;
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(password);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        public async Task<string> DecryptPassword(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;
            string pass = _configuration["PasswordEncryption:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(pass);
            var iv = Encoding.UTF8.GetBytes(pass);

            var encryptedBytes = Convert.FromBase64String(encryptedText);
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentNullException("encryptedText");
            if (keyBytes == null || keyBytes.Length <= 0 || iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            string plaintext = null;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = keyBytes;
                rijAlg.IV = iv;
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    using (var msDecrypt = new MemoryStream(encryptedBytes))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();

                            }
                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }
            return plaintext;
        }

        public async Task<bool> SendMail(List<string> to, List<string>? cc, string subject, string body) // need to use send grid mail here 
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_configuration["MailCredentials:Mail"]);
            foreach (var userMail in to)
            {
                mail.To.Add(new MailAddress(userMail));
            }
            if (cc?.Count > 0)
            {
                foreach (string ccmail in cc)
                {
                    mail.CC.Add(new MailAddress(ccmail));
                }
            }
            mail.Body = body;
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential(_configuration["MailCredentials:Mail"], _configuration["MailCredentials:Password"]);
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void TrimStrings(ref string? filter, ref string? sortColumn, ref string? sortDirection)
        {
            filter = filter?.Trim();
            sortColumn = sortColumn?.Trim();
            sortDirection = sortDirection?.Trim();
        }

        public async Task<bool> SendSms(List<string> phoneNumbers, string body)
        {
            try
            {
                string accountSid = _configuration["TwilioSettings:AccountSid"];
                string authToken = _configuration["TwilioSettings:AuthToken"];
                string fromPhoneNumber = _configuration["TwilioSettings:from"];
                TwilioClient.Init(accountSid, authToken);
                foreach (var phoneNumber in phoneNumbers)
                {
                    var toPhoneNumber = new PhoneNumber(phoneNumber);
                    var message = await MessageResource.CreateAsync(
                       to: toPhoneNumber,
                       from: new PhoneNumber(fromPhoneNumber),
                       body: body
                   );
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> GenerateAccessToken(Users user, bool rememberMe)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_configuration["Jwt:Key"]);
            var signingKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.Role,user.RoleId.ToString())
            };
            var expiration = rememberMe ? DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RememberMeExpiryInDays"])) : DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["Jwt:StandardAccessTokenExpiryHours"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = signingCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken(long userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_configuration["Jwt:Key"]);
            var signingKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var userIdClaim = new Claim("UserId", userId.ToString());
            var expiryHours = Convert.ToInt32(_configuration["Jwt:StandardRefreshTokenExpiryHours"]);
            var expirationTime = DateTime.UtcNow.AddHours(expiryHours);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { userIdClaim }),
                Expires = expirationTime,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> SaveFileAsync(string base64File, string? fileType, string destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                string fileName = Guid.NewGuid().ToString() + "." + fileType;
                byte[] fileBytes = Convert.FromBase64String(base64File);
                string filePath = Path.Combine(destinationFolder, fileName);

                await File.WriteAllBytesAsync(filePath, fileBytes);
                return filePath;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AuthenticationType> GetAuthenticationTypeByIdAsync(long Id)
        {
            try
            {
                var result = await _context.AuthenticationType.Where(a => a.Id == Id).SingleOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OtpRequest> CreateOtpRequestAsync(long userId, string authenticationFor, long authenticationType)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var otp = await GenerateOTP(6);
                    var otpData = new OtpRequest();
                    otpData.UserId = userId;
                    otpData.OtpCode = otp;
                    otpData.ExpirationDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["OtpExpiration"]));
                    otpData.AuthenticationType = authenticationType;
                    otpData.AuthenticationFor = authenticationFor;
                    otpData.CreatedDate = DateTime.UtcNow;
                    otpData.CreatedBy = userId;
                    var result = await _context.OtpRequests.AddAsync(otpData);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result.Entity;
                }
                catch (Exception ex)
                { 
                    await transaction.RollbackAsync();
                    throw;
                }  
            }
        }

        public async Task<string> SendOtpRequestAsync(long userId, string authenticationFor, long authenticationType)
        {
            var userData = await _context.Users.Where(a => a.Id == userId).SingleOrDefaultAsync();

            var authType = await GetAuthenticationTypeByIdAsync(authenticationType);
            var otpData = await CreateOtpRequestAsync(userId, authenticationFor, authenticationType);

            var filePath = authType.AuthType.ToLower() == Constants.EMAIL ? _configuration["Templates:Email"] : _configuration["Templates:Sms"];
            string subject = $"Your Otp Code For {authenticationFor}";
            string body = File.ReadAllText(filePath).Replace("[UserName]", $"{userData.FirstName} {userData.LastName}").Replace("[OTP Code]", otpData.OtpCode).Replace("[X]", _configuration["OtpExpiration"]).Replace("[EventName]", authenticationFor);
            if (authType.AuthType.ToLower() == Constants.EMAIL)
            {
                //var mailResponse = await _sendGridService.SendMailAsync(userData.Email, subject, body, $"{userData.FirstName} {userData.LastName}");
                var toUser = new string[] { userData.Email };
                var mailResponse = await SendMail(toUser.ToList(), null, subject, body);
                //if (!mailResponse.IsSuccess) 
                if(!mailResponse)
                    return Constants.INTERNAL_SERVER_ERROR;
                return Constants.SUCCESS;
            }
            else if (authType.AuthType.ToLower() == Constants.SMS)
            {
                TwilioSendSmsRequest request = new TwilioSendSmsRequest();
                request.Body = body;
                request.To = userData.PhoneNumber;
                var isSmsSent = await _twilioService.SendSmsAsync(request);
                if (!isSmsSent.IsSuccess)
                    return Constants.INTERNAL_SERVER_ERROR;
                return Constants.SUCCESS;
            }
            else if (authType.AuthType.ToLower() == Constants.TWILIO_AUTHY)
            {
                TwilioSendSmsRequest request = new TwilioSendSmsRequest();
                request.Body = body;
                request.To = userData.PhoneNumber;
                var isSent = await _twilioService.Send2FaAsync(userId);
                if (!isSent.IsSuccess)
                    return Constants.INTERNAL_SERVER_ERROR;
                return Constants.SUCCESS;
            }
            return Constants.INTERNAL_SERVER_ERROR;
        }

        public async Task<bool> AddressDetailExistsByIdAsync(long? id)
        {
            var res = await _context.AddressDetails.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return res != null;
        }

        public async Task<long> AddressDetailExistsAsync(string? address, long? cityId, long? region, long? countryId, long? zipcode)
        {
            return await _context.AddressDetails
                .Where(c => c.Address.ToLower() == address.Trim().ToLower() && c.CityId == cityId
                 && c.Region == region && c.CountryId == countryId && c.Zipcode == zipcode).Select(a => a.Id).FirstOrDefaultAsync();
        }

        public async Task<AddressDetail> CreateAddressDetailAsync(AddressDetail entity)
        {
            try
            {
                var result = await _context.AddAsync(entity);
                entity.CreatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteAddressDetailAsync(long id)
        {
            try
            {
                var entity = await _context.AddressDetails.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (entity != null)
                {
                    _context.AddressDetails.Remove(entity);
                    await _context.SaveChangesAsync();
                    return Constants.SUCCESS;
                }
                return Constants.NOT_FOUND;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is MySqlConnector.MySqlException mysqlEx && mysqlEx.Number == Constants.DATA_DEPENDENCY_CODE)
                {
                    return Constants.DATA_DEPENDENCY_ERROR_MESSAGE;
                }
                throw;
            }
        }

        public async Task<AddressDetailResponse> GetAddressDetailByIdAsync(long id)
        {
            try
            {
                var query = from address in _context.AddressDetails
                            join countries in _context.Countries on address.CountryId equals countries.Id
                            join cities in _context.Cities on address.CityId equals cities.Id
                            where address.Id == id
                            select new AddressDetailResponse
                            {
                                Id = address.Id,
                                Address = address.Address,
                                CityId = address.CityId,
                                CityName = cities.Name,
                                Region = address.Region,
                                CountryId = address.CountryId,
                                CountryName = countries.Name,
                                Zipcode = address.Zipcode,
                                IsActive = address.IsActive,
                                CreatedDate = address.CreatedDate
                            };

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponse<AddressDetailResponse>> GetAllAddressDetailAsync(int pageNo, int pageSize, string filter, string sortColumn, string sortDirection, bool isAll = false)
        {
            try
            {
                var query = from address in _context.AddressDetails
                            join countries in _context.Countries on address.CountryId equals countries.Id
                            join cities in _context.Cities on address.CityId equals cities.Id
                            select new AddressDetailResponse
                            {
                                Id = address.Id,
                                Address = address.Address,
                                CityId = address.CityId,
                                CityName = cities.Name,
                                Region = address.Region,
                                CountryId = address.CountryId,
                                CountryName = countries.Name,
                                Zipcode = address.Zipcode,
                                IsActive = address.IsActive,
                                CreatedDate = address.CreatedDate
                            };

                var count = (from address in _context.AddressDetails
                             select address).Count();

                PagedResponse<AddressDetailResponse> pagedResponse = new PagedResponse<AddressDetailResponse>();

                if (isAll)
                {
                    var result = await query.Where(a => a.IsActive == true).ToListAsync();
                    pagedResponse.Result = result;
                    pagedResponse.TotalCount = count;
                    pagedResponse.TotalPages = 0;
                    pagedResponse.CurrentPage = 0;
                    return pagedResponse;
                }

                string orderByExpression = $"{sortColumn} {sortDirection}";

                if (filter != null && filter.Length > 0)
                {
                    filter = filter.ToLower();
                    query = query.Where(x => x.Address.ToLower().Contains(filter));
                    count = query.Count();
                }
                query = query.OrderBy(orderByExpression).Skip((pageNo - 1) * pageSize).Take(pageSize).AsQueryable();

                pagedResponse.Result = query;
                pagedResponse.TotalCount = count;
                pagedResponse.TotalPages = (int)Math.Ceiling((pagedResponse.TotalCount ?? 0) / (double)pageSize);
                pagedResponse.CurrentPage = pageNo;
                return pagedResponse;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<AddressDetail> UpdateAddressDetailAsync(AddressDetail entity)
        {
            try
            {
                _context.AddressDetails.Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GetIpAddress()
        {
            var remoteIpAddress = _contextAccessor?.HttpContext?.Connection.RemoteIpAddress;
            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return remoteIpAddress.ToString();
                }
                else if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    var ipv4 = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
                                 .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    return ipv4.ToString();
                }
            }
            return null;
        }

        public async Task<DateTime> GetStartOfWeek(DateTime date)
        {
            int daysToSubtract = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysToSubtract < 0)
                daysToSubtract += 7;
            return date.AddDays(-daysToSubtract).Date;
        }
        private static string GetFileTypeFromBase64Prefix(string base64String)
        {
            if (base64String.StartsWith("iVBOR"))
                return "png";
            if (base64String.StartsWith("/9j/4"))
                return "jpeg";
            if (base64String.StartsWith("JVBER"))
                return "pdf";
            if (base64String.StartsWith("UEsDB"))
                return "docx";
            return "Unknown";
        }

        public Task<string> HashPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return Task.FromResult(hashedPassword);
            }
        }

        public async Task<long> GetCountryIdByCountryCodeAsync(string countryCode)
        {
            try
            {
                var result = await _context.Countries.Where(a => a.Iso2 == countryCode).Select(a => a.Id).SingleOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GenerateAssitedCode()
        {
            try
            {
                string generatedCode = string.Empty;
                int maxRetries = 5;
                int retryCount = 0;
                do
                {
                    var lastCode = await _context.Patients.OrderByDescending(a => a.Id).Select(a => a.Code).FirstOrDefaultAsync();
                    if (string.IsNullOrEmpty(lastCode))
                        generatedCode = "SIFOPA1000000";
                    else
                    {
                        var lastNumber = Convert.ToInt64(lastCode.Substring(6));
                        var newNumber = lastNumber + 1;
                        generatedCode = $"SIFOPA{newNumber:D7}";
                    }
                    var codeExists = await _context.Patients.AnyAsync(a => a.Code == generatedCode);
                    if (!codeExists)
                        return generatedCode;
                    retryCount++;
                }
                while (retryCount < maxRetries);
                throw new Exception("Error generating assisted code");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating assisted code message {ex.Message} - innerMessage - {ex.InnerException}");
            }
        }
    }
}
