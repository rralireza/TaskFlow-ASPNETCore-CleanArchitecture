using FluentValidation;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using TaskFlow.Application.DTO.Validation;

namespace TaskFlow.API.Middlewares;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning($"Validation exception ocurred: {ex.Message}");

            var errors = ex.Errors.Select(e => new ValidationErrorsDto
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            });

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(errors);

            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception!");

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                Message = "Internal Server Error",
                Details = ex.Message
            });

            await context.Response.WriteAsync(result);
        }
    }

}
