using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ECommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, 
            IConfiguration config, 
            string fileName) where TContext : DbContext
        {
            // Add Generic DbContext
            services.AddDbContext<TContext>(options => options.UseSqlServer(config.GetConnectionString("ECommerceDbConnection"), 
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

            // Configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.txt",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{TimeStamp:yyyy-MM-dd HH:mm:ss:fff zzz} [{Level:u3}] {message:lj} {NewLine} {Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add JWT Authentication Scheme
            JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, config);   

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use Global Exception Middleware
            app.UseMiddleware<Middleware.GlobalExceptions>();
            // Use Listen To Only Api Gateway Middleware
            app.UseMiddleware<Middleware.ListenToOnlyApiGateway>();
            return app;
        }
    }
}
