using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb 
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction) 
        {
            if (isProduction)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"--> Could not run migrations {e.Message}");
                }
                
            }
            
            // if(!context.Platforms.Any())
            // {
            //     Console.WriteLine("Seeding data...");
            //
            //     context.Platforms.AddRange(
            //         new Platform() { Name="Dotnet", Publisher="Microsoft", Cost="Free" },
            //         new Platform() { Name="Sql Server", Publisher="Microsoft", Cost="Free" },
            //         new Platform() { Name="Kubernetes", Publisher="Cloud native computing foundation", Cost="Free" }
            //     );
            //
            //     context.SaveChanges();
            // } 
            // else 
            // {
            //     Console.WriteLine("Already have data");
            // }
        }
    }
}