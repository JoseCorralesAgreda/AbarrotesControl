using FluentValidation;
using AbarrotesControl.Core.CustomEntities;
using AbarrotesControl.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace AbarrotesControl.Api.Filters
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                // El log captura el error completo para el desarrollador
                _logger.LogError(ex, "Excepción capturada por el Middleware Global.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            string message = "Error interno no controlado.";
            object? errors = null;

            switch (exception)
            {
                case BusinessException bizEx:
                    code = bizEx.StatusCode;
                    message = bizEx.Message;
                    errors = bizEx.Details;
                    break;

                case ValidationException valEx:
                    code = HttpStatusCode.BadRequest;
                    message = "Errores de validación en los datos enviados.";
                    errors = valEx.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
                    break;
            }

            var response = new ErrorResponse
            {
                Status = (int)code,
                Title = code.ToString(),
                Message = message,
                Errors = errors,
                TraceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }
    }
}
