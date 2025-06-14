using Microsoft.AspNetCore.Mvc;

namespace UserClient.Infrastructure.Exceptions
{
    public class UserNotFoundException : ProblemDetailsException
    {
        public UserNotFoundException(int userId, string instancePath)
            : base(new ProblemDetails
            {
                Title = "User Not Found",
                Status = 404,
                Detail = $"No user found with ID {userId}.",
                Instance = instancePath
            })
        {
        }
    }
}
