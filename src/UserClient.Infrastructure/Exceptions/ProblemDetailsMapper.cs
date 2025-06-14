// ProblemDetailsMapper.cs
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UserClient.Infrastructure.Exceptions;

namespace UserClient.Infrastructure.ExceptionHandling
{
    public static class ProblemDetailsMapper
    {
        public static ProblemDetailsException Map(Exception ex, string? contextPath = null)
        {
            return ex switch
            {
                HttpRequestException httpEx => new ProblemDetailsException(new ProblemDetails
                {
                    Title = "Network error",
                    Status = 503,
                    Detail = httpEx.Message,
                    Instance = contextPath
                }),

                TaskCanceledException timeoutEx => new ProblemDetailsException(new ProblemDetails
                {
                    Title = "Request timeout",
                    Status = 408,
                    Detail = timeoutEx.Message,
                    Instance = contextPath
                }),

                JsonException jsonEx => new ProblemDetailsException(new ProblemDetails
                {
                    Title = "Invalid response format",
                    Status = 500,
                    Detail = jsonEx.Message,
                    Instance = contextPath
                }),

                _ => new ProblemDetailsException(new ProblemDetails
                {
                    Title = "Unexpected error",
                    Status = 500,
                    Detail = ex.Message,
                    Instance = contextPath
                }),
            };
        }

        public static ProblemDetailsException CreateUserNotFound(int userId, string instancePath)
        {
            return new ProblemDetailsException(new ProblemDetails
            {
                Title = "User Not Found",
                Status = 404,
                Detail = $"No user found with ID {userId}.",
                Instance = instancePath
            });
        }
    }
}
