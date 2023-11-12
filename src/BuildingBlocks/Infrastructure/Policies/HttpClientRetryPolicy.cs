using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies
{
    public static class HttpClientRetryPolicy
    {
        public static void UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder, int retryAttempt = 3)
        {
            builder.AddPolicyHandler(ImmediateHttpRetry(retryAttempt));
        }

        public static void UseLinearHttpRetryPolicy(this IHttpClientBuilder builder, int retryAttempt = 3, int fromSeconds = 30)
        {
            builder.AddPolicyHandler(LinearHttpRetry(retryAttempt, fromSeconds));
        }

        public static void UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder, int retryAttempt = 5)
        {
            builder.AddPolicyHandler(ExponentialHttpRetry(retryAttempt));
        }

        private static IAsyncPolicy<HttpResponseMessage> ImmediateHttpRetry(int retryAttempt) =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(retryAttempt, (exception, retryCount, context) =>
                {
                    Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
                });

        private static IAsyncPolicy<HttpResponseMessage> LinearHttpRetry(int retryAttempt, int fromSeconds) =>
           HttpPolicyExtensions
               .HandleTransientHttpError()
               .WaitAndRetryAsync(retryAttempt, _ => TimeSpan.FromSeconds(fromSeconds), (exception, retryCount, context) =>
               {
                   Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
               });

        private static IAsyncPolicy<HttpResponseMessage> ExponentialHttpRetry(int retryAttempt) =>
         HttpPolicyExtensions
             .HandleTransientHttpError()
             .WaitAndRetryAsync(retryAttempt, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, retryCount, context) =>
             {
                 Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
             });
    }
}
