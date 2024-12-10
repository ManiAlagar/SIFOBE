using Microsoft.EntityFrameworkCore;
using SIFO.APIService.Authentication;
using SIFO.Model.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddPresentation(builder.Configuration).
    AddInfrastructure(builder.Configuration, builder.Environment);
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
app.UseMiddleware<OtpValidationMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler("/error");

app.MapControllers();

app.Run();
