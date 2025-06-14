// File: Exceptions/ProblemDetailsException.cs
using Microsoft.AspNetCore.Mvc;

namespace UserClient.Infrastructure.Exceptions
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetails ProblemDetails { get; }

        public ProblemDetailsException(ProblemDetails problemDetails)
            : base(problemDetails.Detail)
        {
            ProblemDetails = problemDetails;
        }
    }
}
