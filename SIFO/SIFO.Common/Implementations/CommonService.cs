using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace SIFO.Utility.Implementations
{
    public class CommonService : ICommonService
    {
        private readonly IConfiguration _configuration;
        SIFOContext _context;
        IHttpContextAccessor _contextAccessor;
        public CommonService(IConfiguration configuration, SIFOContext context, IHttpContextAccessor contextAccessor)
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            _context = context;
            _contextAccessor = contextAccessor;
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
                Role = claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value,
                FullName = claims.FirstOrDefault(c => c.Type == "UserName")?.Value,
                Email = claims.FirstOrDefault(c => c.Type == "Email")?.Value
            };
            return user;
        }

        public async Task<string> EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            using (var aes = Aes.Create())
            {
                string SecretKey = _configuration["PasswordEncryption:Key"];
                aes.Key = Encoding.UTF8.GetBytes(SecretKey);
                aes.GenerateIV();
                var iv = aes.IV;

                using (var ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);
                    using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(password);
                            }
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public async Task<string> DecryptPassword(string encryptedText)
        {
            var fullCipher = Convert.FromBase64String(encryptedText);
            var iv = new byte[16];
            Array.Copy(fullCipher, iv, iv.Length);

            var cipher = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using (var aes = Aes.Create())
            {
                string SecretKey = _configuration["PasswordEncryption:Key"];
                aes.Key = Encoding.UTF8.GetBytes(SecretKey);
                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                {
                    using (var ms = new MemoryStream(cipher))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
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
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> GenerateAccessToken(User user, bool rememberMe)
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
    }
}
