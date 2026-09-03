
using AutomoveisVendasApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace automoveisVendasApi.Exceptions
{

    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;

            var (statusCode, title) = MapException(exception);

           
            _logger.LogError(
                exception,
                "Exceção não tratada capturada pelo GlobalExceptionHandler. TraceId: {TraceId}, StatusCode: {StatusCode}",
                traceId, statusCode);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = $"https://httpstatuses.io/{statusCode}",
                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "Ocorreu um erro ao processar a sua requisição.",
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] = traceId;

            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
        {
            ResourceNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito de estado"),
            DomainException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };
    }
}