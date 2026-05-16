using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductApi.Infrastructure.Data
{
    public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ECommerceDbConnection")
                ?? GetConnectionStringFromAppSettings();

            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());

            return new ProductDbContext(optionsBuilder.Options);
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
                    "ECommerce.ProductApiSolution",
                    "ProductApi.Presentation",
                    "appsettings.json");

                if (File.Exists(appSettingsPath))
                {
                    return appSettingsPath;
                }

                appSettingsPath = Path.Combine(
                    currentDirectory.FullName,
                    "ProductApi.Presentation",
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

            throw new FileNotFoundException("Could not find ProductApi.Presentation appsettings.json.");
        }
    }
}
