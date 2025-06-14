using Polly;
using Polly.Extensions.Http;

namespace UserClient.Infrastructure.Policies
{
    public static class UserClientPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"[Polly] Retry {retryAttempt} after {timespan.TotalSeconds}s.");
                    });
        }
    }
}
