using FluentValidation.AspNetCore;
using OkxPerpetualArbitrage.Application;
using OkxPerpetualArbitrage.OkxApi;
using OkxPerpetualArbitrage.Persistance;
using OkxPerpetualArbitrage.WebApi.BackgroundServices;
using OkxPerpetualArbitrage.WebApi.Middlewares;
using Serilog;

Log.Logger = new LoggerConfiguration().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));



// Add services to the container.

builder.Services.AddControllers().AddFluentValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ApplicationServicesRegistration(builder.Configuration);
builder.Services.PersistanceServicesRegistration(builder.Configuration);
builder.Services.ApiServicesRegistration(builder.Configuration);


//Enable background workers
 builder.Services.AddHostedService<FundsUpdaterService>();
// builder.Services.AddHostedService<PotentialPositionUpdaterService>();
// builder.Services.AddHostedService<CleanerService>();


builder.Services.AddHostedService<PositionOpenerService>();
builder.Services.AddHostedService<PositionCloserService>();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();


app.ConfigureExceptionHandler();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("corsapp");
app.UseAuthorization();

app.MapControllers();

app.Run();
