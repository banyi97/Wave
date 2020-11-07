using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Wave.Services
{
    public static class DbContextMigrate
    {
        public static IHost MigrateDbContext<TContext>(this IHost host) where TContext : DbContext
        {

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<TContext>();
                if (dbContext.Database.IsSqlServer())
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Migration error");

                throw;
            }

            return host;
        }

    }
}
