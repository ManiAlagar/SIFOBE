using Serilog;
using SIFO.Model;
using SIFO.Model.Entity;
using SIFO.Core.MiddleWare;
using SIFO.APIService.Hospital;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Services.AddGrpc();

// Clear default logging providers and add Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add necessary services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register other services (Infrastructure, Model extensions, etc.)
builder.Services
    .AddInfrastructure(builder.Configuration, builder.Environment)
    .AddModelExtensions();

// Add DbContext with MySQL configuration
builder.Services.AddDbContext<SIFOContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25))));

var app = builder.Build();

// Configure middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Exception handling middleware should be first
app.UseMiddleware<ExceptionHandlingMiddleware>();

// CORS setup should come before routing
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Security middleware
app.UseHttpsRedirection();
app.UseAuthentication();


// Routing must be before endpoints
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<PharmacyServiceImpl>();
});

// Map controllers for API endpoints
app.MapControllers();

app.Run();
