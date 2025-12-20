using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace DAL.DbContext
{
    public static class DatabaseProviderResolver
    {
        public static void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var provider = (options.Provider ?? "Postgres").Trim().ToLowerInvariant();

            if (provider == "sqlite")
            {
                var path = Path.GetFullPath(options.SqlitePath);
                builder.UseSqlite($"Data Source={path}");
            }
            else
            {
                builder.UseNpgsql(options.ConnectionString);
            }

            if (options.EnableSensitiveLogging)
            {
                builder.EnableSensitiveDataLogging();
            }
        }
    }
}
