using Serilog;
using Microsoft.EntityFrameworkCore;
using SIFOYarpGateway.Models;
using Yarp.ReverseProxy.Configuration;
using Microsoft.AspNetCore.HttpOverrides;

namespace SIFOYarpGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            builder.Services.AddControllers();

            
            builder.Services.AddDbContext<GatewayDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


            
            ServiceCollectionExtension.ConfigureServices(builder.Services);

            var app = builder.Build();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();
            app.MapControllers();
            app.MapReverseProxy();

            app.Run();
        }
    }
}
