using SendGrid;
using SIFO.Common.Contracts;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SIFO.APIService.Authentication.Repository.Contracts;
using SIFO.APIService.Authentication.Repository.Implementations;
using SIFO.APIService.Authentication.Service.Contracts;
using SIFO.Core.Service.Contracts;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Service.Implementations;
using SIFO.Core.Repository.Implementations;

namespace SIFO.APIService.Authentication
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            var jwtSettings = new JwtSettings();
            configuration.Bind(JwtSettings.SectionName, jwtSettings);
            services.AddSingleton(Options.Create(jwtSettings));
            services.AddTransient<JwtTokenGenerator>();
            services.AddTransient<IAuthenticationService,Service.Implementations.AuthenticationService>();
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            services.AddTransient<ICommonService,CommonService>();
            services.AddTransient<SIFOContext>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddTransient<ITwilioService, TwilioService>();
            services.AddTransient<ITwilioRepository, TwilioRepository>();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
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
