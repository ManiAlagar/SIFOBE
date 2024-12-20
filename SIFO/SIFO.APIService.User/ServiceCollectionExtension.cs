using Microsoft.Extensions.Options;
using SIFO.APIService.User.Service.Contracts;
using SIFO.APIService.User.Service.Implementations;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SIFO.APIService.User.Repository.Contracts;
using SIFO.APIService.User.Repository.Implementations;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using Microsoft.OpenApi.Models;
using FluentValidation;
using System.Reflection;
using SIFO.Model.AutoMapper; 
using SendGrid;
using SIFO.Core.Service.Contracts;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Service.Implementations;
using SIFO.Core.Repository.Implementations;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.APIService.Master.Service.Implementations;
using SIFO.APIService.Master.Repository.Implementations;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Repository.Implementations;

namespace SIFO.APIService.User
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,ConfigurationManager configuration,IWebHostEnvironment environment)
        {
            var jwtSettings = new JwtSettings();
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            configuration.Bind(JwtSettings.SectionName, jwtSettings);
            services.AddSingleton(Options.Create(jwtSettings));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //mapper 
            services.AddAutoMapper(typeof(MapperProfile).Assembly);
            //Services 
            services.AddTransient<SIFOContext>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddTransient<ITwilioService, TwilioService>();
            services.AddTransient<ITwilioRepository, TwilioRepository>();
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<IPharmacyRepository, PharmacyRepository>();
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
