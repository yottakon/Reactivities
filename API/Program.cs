using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
//using Microsoft.Extensions.DependencyInjection.Abstractions.dll;

namespace API
{
    public class Program
    {   
        //name method async Task so it owuld return a task, and allow await calls and async calls
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //create a scope that hosts any services
            //Using lets this scope be used once and disposed of
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            try
            {
                //This is getting the DataContext
                //Creates a database if one doesn't exist
                 var context = services.GetRequiredService<DataContext>();
                 var userManager = services.GetRequiredService<UserManager<AppUser>>();
                 await context.Database.MigrateAsync();
                 await Seed.SeedData(context, userManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            //make sure to run host in the end
            //This runs the application
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
