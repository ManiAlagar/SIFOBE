using FluentValidation;
using System.Text.Json;
using SIFO.Model.Request;
using SIFO.Model.Validator;
using SIFO.Model.AutoMapper;
using FluentValidation.AspNetCore;
using System.Diagnostics.CodeAnalysis;
using SIFO.APIService.Master.Service.Contracts;
using SIFO.APIService.Master.Repository.Contracts;
using SIFO.APIService.Master.Service.Implementations;
using SIFO.APIService.Master.Repository.Implementations;
using SIFO.Common.Contracts;
using SIFO.Utility.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SIFO.Model.Response;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace SIFO.APIService.Master
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(JwtSettings.SectionName, jwtSettings);
            //Validators
            //services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //}).AddFluentValidation();
            services.AddHttpContextAccessor();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //services.AddSingleton<IValidator<CountryRequest>, CountryValidator>();
            //services.AddSingleton<IValidator<StateRequest>, StateValidator>();
            //services.AddSingleton<IValidator<CityRequest>, CityValidator>();

            //Mapper
            services.AddAutoMapper(typeof(MapperProfile).Assembly);

            //Repositories
            services.AddTransient<ICountryRepository, CountryRepository>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<IStateRepository, StateRepository>();
            services.AddTransient<IAddressRepository, AddressRepository>();

            //Services
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IMasterService, MasterService>();
            services.AddTransient<IMasterRepository, MasterRepository>();
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<ICommonService, CommonService>();
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
