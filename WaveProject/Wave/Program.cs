using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Data.SqlClient;
using Wave.Database;
using System.Runtime.CompilerServices;
using Wave.Services;

namespace Wave
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();
            try
            {
                Log.Information("Starting up");
                var build = CreateHostBuilder(args).Build();                            
                try
                {
                    build.MigrateDbContext<ApplicationDbContext>();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Database migration failed");
                    throw ex;
                }
                build.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                //    config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true);
                //    config.AddEnvironmentVariables();
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
