
using Microsoft.EntityFrameworkCore;
using SIFO.Model.Entity;

namespace SIFO.APIService.User
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
            builder.Services.AddDbContext<SIFOContext>(options =>
             options.UseMySql(
             builder.Configuration.GetConnectionString("DefaultConnection"),
             new MySqlServerVersion(new Version(8, 0, 25))));
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<OtpValidationMiddleware>();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
