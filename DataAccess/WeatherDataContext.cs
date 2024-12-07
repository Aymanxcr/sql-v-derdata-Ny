using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Väderdata.DataAccess
{
    public class WeatherDataContext : DbContext
    {
        // Default constructor
        public WeatherDataContext()
        {
        }

        // Constructor accepting DbContextOptions for dependency injection
        public WeatherDataContext(DbContextOptions<WeatherDataContext> options) : base(options)
        {
        }

        // DbSet property representing the Weather table in the database
        public DbSet<WeatherModel>? Weather { get; set; }

        // Override the OnConfiguring method to configure the database connection
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Check if the optionsBuilder is already configured
            if (!optionsBuilder.IsConfigured)
            {
                // Attempted configuration using appsettings.json (commented out)
                // This approach could be used in a production environment
                // if configuration files are available.

                /*
                var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);
                string connectionString =
                    builder.Build().GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
                */

                // Fallback to hardcoded connection string for local development
                // Specifies the local SQL Server database instance and database name
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WeatherData;Trusted_Connection=True;");
            }
        }
    }
}
