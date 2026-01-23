using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;


namespace UniversityManagement.Infrastructure.Data
{
    public class UniversityDbContextFactory : IDesignTimeDbContextFactory<UniversityDbContext>
    {
        public UniversityDbContext CreateDbContext(string[] args)
        {
            // Try to load appsettings.json from the API project folder
            var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "UniversityManagement.Api");

            var basePath = Directory.Exists(apiPath)
                ? apiPath
                : Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new UniversityDbContext(optionsBuilder.Options);
        }
    }
}
