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
            switch (ex)
            {
                case UserNotFoundException notFound:
                    return notFound;

                case HttpRequestException httpEx:
                    return new ProblemDetailsException(new ProblemDetails
                    {
                        Title = "Network error",
                        Status = 503,
                        Detail = httpEx.Message,
                        Instance = contextPath
                    });

                case TaskCanceledException timeoutEx:
                    return new ProblemDetailsException(new ProblemDetails
                    {
                        Title = "Request timeout",
                        Status = 408,
                        Detail = timeoutEx.Message,
                        Instance = contextPath
                    });

                case JsonException jsonEx:
                    return new ProblemDetailsException(new ProblemDetails
                    {
                        Title = "Invalid response format",
                        Status = 500,
                        Detail = jsonEx.Message,
                        Instance = contextPath
                    });

                default:
                    return new ProblemDetailsException(new ProblemDetails
                    {
                        Title = "Unexpected error",
                        Status = 500,
                        Detail = ex.Message,
                        Instance = contextPath
                    });
            }
        }

        
    }
}
