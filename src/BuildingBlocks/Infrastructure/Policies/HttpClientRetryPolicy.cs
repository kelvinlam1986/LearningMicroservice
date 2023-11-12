using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace Infrastructure.Policies
{
    public static class HttpClientRetryPolicy
    {
        public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder, int retryAttempt = 3)
        {
            return builder.AddPolicyHandler(ImmediateHttpRetry(retryAttempt));
        }

        public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder, int retryAttempt = 3, int fromSeconds = 30)
        {
           return builder.AddPolicyHandler(LinearHttpRetry(retryAttempt, fromSeconds));
        }

        public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder, int retryAttempt = 5)
        {
            return builder.AddPolicyHandler(ExponentialHttpRetry(retryAttempt));
        }

        public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder, int handledEventsAllowedBeforeBreaking = 3, int durationOfBreak = 30)
        {
            return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy(handledEventsAllowedBeforeBreaking, durationOfBreak));
        }

        public static IHttpClientBuilder ConfigureTimeoutPolicy(this IHttpClientBuilder builder, int seconds= 5) 
        {
            return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(seconds));
        }

        private static IAsyncPolicy<HttpResponseMessage> ImmediateHttpRetry(int retryAttempt) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .RetryAsync(retryAttempt, (exception, retryCount, context) =>
                {
                    Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
                });

        private static IAsyncPolicy<HttpResponseMessage> LinearHttpRetry(int retryAttempt, int fromSeconds) =>
           HttpPolicyExtensions
               .HandleTransientHttpError()
               .Or<TimeoutRejectedException>()
               .WaitAndRetryAsync(retryAttempt, _ => TimeSpan.FromSeconds(fromSeconds), (exception, retryCount, context) =>
               {
                   Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
               });

        private static IAsyncPolicy<HttpResponseMessage> ExponentialHttpRetry(int retryAttempt) =>
         HttpPolicyExtensions
             .HandleTransientHttpError()
             .Or<TimeoutRejectedException>()
             .WaitAndRetryAsync(retryAttempt, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, retryCount, context) =>
             {
                 Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
             });

        private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, int durationOfBreak)
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
                        durationOfBreak: TimeSpan.FromSeconds(durationOfBreak));
        }
    }
}
