using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Repository.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Service.Implementations;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Request;
using SIFO.Model.Response;
using SIFO.Model.Validator;
using SIFO.Utility.Implementations;
using System.Text;
using System.Text.Json;

namespace SIFO.APIService.Hospital
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(
           this IServiceCollection services,
           ConfigurationManager configuration,
           IWebHostEnvironment environment
       )
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(JwtSettings.SectionName, jwtSettings);
            services.AddSingleton(Options.Create(jwtSettings));
            services.AddTransient<IHospitalService,HospitalService>();
            services.AddTransient<IHospitalRepository, HospitalRepository>();
            services.AddTransient<ICommonService,CommonService>();
            services.AddTransient<SIFOContext>();
            services.AddHttpContextAccessor();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
                options.UseSecurityTokenValidators = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero // Optional: adjust if needed
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // Log the exception or handle it as needed
                        var exception = context.Exception;
                        Console.WriteLine(exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // Additional validation if needed
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
