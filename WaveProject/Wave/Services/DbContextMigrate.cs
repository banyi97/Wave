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
            //https://anduin.aiursoft.com/post/2019/12/14/auto-update-database-for-aspnet-core-with-entity-framework
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                var config = services.GetService<IConfiguration>();
                //var res = context.Database.EnsureCreated();
                //var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
                //var isConnected = false;
                //do
                //{
                //    try
                //    {
                //        connection.Open();
                //        isConnected = true;
                //    }
                //    catch (Exception ex) {
                //        Log.Information(ex.Message);
                //    }
                //    Thread.Sleep(1000);
                //} while (!isConnected);
                //connection.Close();
                //connection.Dispose();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database.Migrate();
                
                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return host;
        }

    }
}
