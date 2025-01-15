using SendGrid;
using System.Text;
using SIFO.Model.Entity;
using SIFO.Model.Response;
using SIFO.Common.Contracts;
using SIFO.Model.AutoMapper;
using Microsoft.OpenApi.Models;
using SIFO.Core.Service.Contracts;
using SIFO.Utility.Implementations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SIFO.Core.Repository.Contracts;
using SIFO.Core.Service.Implementations;
using SIFO.Core.Repository.Implementations;
using SIFO.APIService.Hospital.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SIFO.APIService.Hospital.Repository.Contracts;
using SIFO.APIService.Hospital.Service.Implementations;
using SIFO.APIService.Hospital.Repository.Implementations;

namespace SIFO.APIService.Hospital
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,ConfigurationManager configuration,IWebHostEnvironment environment)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(JwtSettings.SectionName, jwtSettings);
            var sendGridApiKey = configuration["SendGridSettings:ApiKey"];
            services.AddSingleton(Options.Create(jwtSettings));

            //Mapper 
            services.AddAutoMapper(typeof(MapperProfile).Assembly);

            //Services
            services.AddTransient<ICommonService,CommonService>();
            services.AddTransient<IHospitalService,HospitalService>();
            services.AddTransient<IPharmacyService,PharmacyService>();
            services.AddTransient<IHospitalFacilityService, HospitalFacilityService>();
            services.AddTransient<ITwilioService, TwilioService>();
            services.AddTransient<ISendGridService, SendGridService>();
            services.AddTransient<IDrugService, DrugService>();

            //Repositories
            services.AddTransient<IHospitalRepository, HospitalRepository>();
            services.AddTransient<IPharmacyRepository,PharmacyRepository>();
            services.AddTransient<IHospitalFacilityRepository, HospitalFacilityRepository>();
            services.AddTransient<ITwilioRepository, TwilioRepository>();
            services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));
            services.AddTransient<IDrugRepository,DrugRepository>();

            //DbContext
            services.AddTransient<SIFOContext>();

            //Others
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

            return services;
        }
    }
}
