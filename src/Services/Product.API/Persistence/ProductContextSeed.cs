using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using ILogger = Serilog.ILogger;

namespace Product.API.Persistence
{
    public static class ProductContextSeed
    {
        public static async Task SeedProductAsync(ProductContext productContext, ILogger logger)
        {
            if (!await productContext.Products.AnyAsync()) 
            {
                await productContext.Products.AddRangeAsync(GetProductCatalogs());
                await productContext.SaveChangesAsync();
                logger.Information("Seeded data for ProductDB associated with context {DBContextName}",
                    nameof(productContext));
            }
        }

        private static IEnumerable<CatalogProduct> GetProductCatalogs()
        {
            return new List<CatalogProduct>()
            {
                new CatalogProduct
                {
                    No = "P1",
                    Name = "Product 1",
                    Summary = "Product 1",
                    Description = "Description Product 1",
                    Price = 50000,
                },
                new CatalogProduct
                {
                    No = "P2",
                    Name = "Product 2",
                    Summary = "Product 2",
                    Description = "Description Product 2",
                    Price = 70000,
                },
            };
        }
    }
}
