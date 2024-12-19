
using Microsoft.EntityFrameworkCore;
using Serilog;
using SIFO.Core.MiddleWare;
using SIFO.Model;
using SIFO.Model.Entity;

namespace SIFO.APIService.User
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
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructure(builder.Configuration, builder.Environment).AddModelExtensions();
            builder.Services.AddDbContext<SIFOContext>(options =>
             options.UseMySql(
             builder.Configuration.GetConnectionString("DefaultConnection"),
             new MySqlServerVersion(new Version(8, 0, 25))));
            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseHttpsRedirection();
          //  app.UseMiddleware<OtpValidationMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
