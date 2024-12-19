using FluentValidation;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SIFO.Model
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddModelExtensions(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
