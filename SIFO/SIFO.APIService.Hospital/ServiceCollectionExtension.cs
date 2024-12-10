using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Repository.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using SIFO.APIService.Hospital.Service.Implementations;
using SIFO.Common.Contracts;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Repository.Implementations;
using SIFO.Core.Service.Contracts;
using SIFO.Core.Service.Implementations;
using SIFO.Model.AutoMapper;
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
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            services.AddSingleton(Options.Create(jwtSettings));

            //mapper 
            services.AddAutoMapper(typeof(MapperProfile).Assembly);

            services.AddTransient<IHospitalService,HospitalService>();
            services.AddTransient<IHospitalRepository, HospitalRepository>();
            services.AddTransient<ICommonService,CommonService>();
            services.AddTransient<SIFOContext>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddTransient<ITwilioService, TwilioService>();
            services.AddTransient<ITwilioRepository, TwilioRepository>();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Here Enter JWT token in bearer format"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                           Reference= new OpenApiReference
                           {
                               Type=ReferenceType.SecurityScheme,
                               Id="Bearer"
                           }
                        },
                        new string[]{}
                    }
               });
            });
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
                    ClockSkew = TimeSpan.Zero 
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var exception = context.Exception;
                        Console.WriteLine(exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
