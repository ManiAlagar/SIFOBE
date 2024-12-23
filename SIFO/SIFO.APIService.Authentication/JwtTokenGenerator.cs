using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.Security.Claims;
using System.Text;

namespace SIFO.APIService.Authentication
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public string GenerateToken(Users user)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256
            );

            List<Claim> claims = new();
            Claim Email = new Claim("Email", user.Email);
            Claim UserId = new Claim("UserId", user.Id.ToString());
            Claim RoleId = new Claim("RoleId", user.RoleId.ToString());
            Claim UserName = new Claim("UserName",$"{user.FirstName} {user.LastName}");
            Claim Roles = new Claim(ClaimTypes.Role, user.RoleName);
            Claim ParentRoleId = new Claim("ParentRoleId", string.Join(",",user.ParentRole));
            claims.Add(Email);
            claims.Add(UserId);
            claims.Add(UserName);
            claims.Add(Roles);
            claims.Add(RoleId);
            claims.Add(ParentRoleId);

            var securityToken = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                            SecurityAlgorithms.HmacSha256)
            };

            var token = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler().CreateToken(securityToken);
            return token;
        }
    }
}
