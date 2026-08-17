using CatalogService.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Catalog request failed.");
            await WriteProblemDetailsAsync(context, ex);
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
    {
        var problem = exception switch
        {
            ValidationException validationException => new ProblemDetails
            {
                Title = "Validation failed",
                Detail = string.Join("; ", validationException.Errors.Select(error => error.ErrorMessage)),
                Status = StatusCodes.Status400BadRequest
            },
            NotFoundException => new ProblemDetails { Title = "Resource not found", Detail = exception.Message, Status = StatusCodes.Status404NotFound },
            ConflictException or DbUpdateException => new ProblemDetails { Title = "Request conflict", Detail = exception.Message, Status = StatusCodes.Status409Conflict },
            ArgumentException => new ProblemDetails { Title = "Validation failed", Detail = exception.Message, Status = StatusCodes.Status400BadRequest },
            _ => new ProblemDetails { Title = "Unexpected error", Detail = "An unexpected error occurred.", Status = StatusCodes.Status500InternalServerError }
        };

        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }
}
