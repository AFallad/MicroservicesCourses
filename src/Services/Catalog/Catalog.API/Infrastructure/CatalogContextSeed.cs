using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Models.DomainModels;
using Microsoft.Data.SqlClient;
using Polly;
using Serilog;

namespace Catalog.API.Infrastructure
{
    public class CatalogContextSeed
    {
        public List<CatalogBrand> Brands = new List<CatalogBrand> {new CatalogBrand {Id = 1, Brand = "Accenture"}};
        public List<CatalogType> Types = new List<CatalogType> {new CatalogType {Id = 1, Type = "Shirt"}};

        public async Task SeedAsync(CatalogDbContext context)
        {
            var policy = Policy.Handle<SqlException>().WaitAndRetryAsync(
                5,
                retry => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retry, ctx) =>
                {
                    Log.Error(exception,
                        "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                        nameof(CatalogContextSeed), exception.GetType().Name, exception.Message, retry, 5);
                }
            );

            await policy.ExecuteAsync(async () =>
            {
                if (!context.CatalogBrands.Any())
                {
                    await context.CatalogBrands.AddRangeAsync(this.Brands);
                    await context.SaveChangesAsync();
                }

                if (!context.CatalogBrands.Any())
                {
                    await context.CatalogTypes.AddRangeAsync(this.Types);
                    await context.SaveChangesAsync();
                }
            });
        }
    }
}