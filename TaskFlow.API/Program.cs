using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Middlewares;
using TaskFlow.API.StartupExtensions;
using TaskFlow.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigurationServices(builder.Configuration);

var app = builder.Build();

// Keep a fresh SQL Server volume usable without a separate migration command.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ValidationExceptionMiddleware>();

app.Run();
