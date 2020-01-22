using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Catalog.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var ctx = services.GetService<TContext>();

                try
                {
                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        });
                    logger.LogInformation("Migrating context {DbContextName}", typeof(TContext).Name);
                    retry.Execute(() => MigrateAndSeed(seeder, ctx, services));
                    logger.LogInformation("Migrated context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "An error occurred while migrating the database used on context {DbContextName}",
                        typeof(TContext).Name);
                }
            }

            return host;
        }

        public static void MigrateAndSeed<TContext>(Action<TContext, IServiceProvider> seeder, TContext ctx, IServiceProvider services) where TContext : DbContext
        {
            ctx.Database.Migrate();
            seeder(ctx, services);
        }
    }
}