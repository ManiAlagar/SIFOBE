using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid;
using SIFO.APIService.Patient.Repository.Contracts;
using SIFO.APIService.Patient.Repository.Implementations;
using SIFO.APIService.Patient.Service.Contracts;
using SIFO.APIService.Patient.Service.Implementations;
using SIFO.Common.Contracts;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Repository.Implementations;
using SIFO.Core.Service.Contracts;
using SIFO.Core.Service.Implementations;
using SIFO.Model.AutoMapper;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Utility.Implementations;
using System.Text;

namespace SIFO.APIService.Hospital
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(JwtSettings.SectionName, jwtSettings);
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            services.AddSingleton(Options.Create(jwtSettings));

            //Mapper 
            services.AddAutoMapper(typeof(MapperProfile).Assembly);

            #region Services
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IIntoleranceManagementService, IntoleranceManagementService>();
            services.AddTransient<ITherapeuticPlanService, TherapeuticPlanService>();
            services.AddTransient<IAllergyService, AllergyService>();
            services.AddTransient<IWeeklyMoodEntryService, WeeklyMoodEntryService>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddTransient<ITwilioService, TwilioService>(); 
            services.AddTransient<IAdverseEventService, AdverseEventService>(); 
            services.AddTransient<IPatientAnalysisReportService,PatientAnalysisReportService>(); 
            #endregion

            #region Repositories
            services.AddTransient<IIntoleranceManagementRepository, IntoleranceManagementRepository>();
            services.AddTransient<ITherapeuticPlanRepository, TherapeuticPlanRepository>();
            services.AddTransient<IWeeklyMoodEntryRepository, WeeklyMoodEntryRepository>();
            services.AddTransient<IAllergyRepository, AllergyRepository>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddTransient<ITwilioRepository, TwilioRepository>(); 
            services.AddTransient<IAdverEventRepository, AdverseEventRepository>(); 
            services.AddTransient<IPatientAnalysisReportRepository, PatientAnalysisReportRepository>(); 
            #endregion

            //DbContext
            services.AddTransient<SIFOContext>();

            #region Others
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
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
            #endregion

            return services;
        }
    }
}
