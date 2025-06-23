using Scalar.AspNetCore;
using TaskFlow.API.Middlewares;
using TaskFlow.API.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigurationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ValidationExceptionMiddleware>();

app.Run();
