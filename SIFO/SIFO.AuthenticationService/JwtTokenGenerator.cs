﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIFO.AuthenticationService
{
    public class JwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(
            IOptions<JwtSettings> jwtOptions
        )
        {
            _jwtSettings = jwtOptions.Value;
        }

        public string GenerateToken(User user)
        {


            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256
            );

            List<Claim> claims = new();
            Claim sub = new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString());
            Claim UserName = new Claim("Email", user.Email);
            Claim UserId = new Claim("UserId", user.Id.ToString());
            Claim GivenName = new Claim(JwtRegisteredClaimNames.GivenName, user.Email);
            Claim Jti = new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            Claim Roles = new Claim(ClaimTypes.Role, user.RoleId.ToString());
            claims.Add(sub);
            claims.Add(UserName);
            claims.Add(UserId);
            claims.Add(GivenName);
            claims.Add(Jti);
            claims.Add(Roles);
            //if (user.RoleNames != null && user.RoleNames.Count() > 0)
            //{
            //    foreach (string role in user.RoleNames)
            //    {
            //        claims.Add(new Claim("Roles", role.ToString()));
            //    }
            //}

            var securityToken = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
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
            Console.WriteLine($"Generated JWT Token: {token}"); 
            return token;
        }
    }
}
