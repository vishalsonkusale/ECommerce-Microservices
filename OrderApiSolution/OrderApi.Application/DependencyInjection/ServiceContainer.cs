using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            // Register Application Services
            services.AddHttpClient<IOrderService, OrderService>(client =>
            {
                client.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                client.Timeout = TimeSpan.FromSeconds(1);
            });


            // Create retry Strategy
            var retryStrategy = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500)
            };

            // Use retry strategy in resilience pipeline
            services.AddResiliencePipeline("RetryPipleline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });

            return services;
        }
    }
}
