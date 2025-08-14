using Polly;
using Polly.Extensions.Http;

namespace ThreadPilot.InsuranceService.Configuration;

public static class HttpPolicyConfiguration
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger? logger = null)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, _) =>
                {
                    logger?.LogWarning("HTTP retry {RetryCount} after {Delay}ms",
                        retryCount, timespan.TotalMilliseconds);
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger? logger = null)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (_, duration) =>
                {
                    logger?.LogWarning("Circuit breaker opened for {Duration}ms", duration.TotalMilliseconds);
                },
                onReset: () =>
                {
                    logger?.LogInformation("Circuit breaker reset");
                });
    }
}