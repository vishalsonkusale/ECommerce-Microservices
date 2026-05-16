using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderApi.Infrastructure.Data
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ECommerceDbConnection")
                ?? GetConnectionStringFromAppSettings();

            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());

            return new OrderDbContext(optionsBuilder.Options);
        }

        private static string GetConnectionStringFromAppSettings()
        {
            var appSettingsPath = FindAppSettingsPath();
            using var appSettings = JsonDocument.Parse(File.ReadAllText(appSettingsPath));

            var connectionString = appSettings.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("ECommerceDbConnection")
                .GetString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'ECommerceDbConnection' was not found.");
            }

            return connectionString;
        }

        private static string FindAppSettingsPath()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (currentDirectory is not null)
            {
                var appSettingsPath = Path.Combine(
                    currentDirectory.FullName,
                    "OrderApiSolution",
                    "OrderApi.Presentation",
                    "appsettings.json");

                if (File.Exists(appSettingsPath))
                {
                    return appSettingsPath;
                }

                appSettingsPath = Path.Combine(
                    currentDirectory.FullName,
                    "OrderApi.Presentation",
                    "appsettings.json");

                if (File.Exists(appSettingsPath))
                {
                    return appSettingsPath;
                }

                appSettingsPath = Path.Combine(currentDirectory.FullName, "appsettings.json");

                if (File.Exists(appSettingsPath))
                {
                    return appSettingsPath;
                }

                currentDirectory = currentDirectory.Parent;
            }

            throw new FileNotFoundException("Could not find OrderApi.Presentation appsettings.json.");
        }
    }
}
