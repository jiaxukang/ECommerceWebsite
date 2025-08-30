using System;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreSeedData
{
    public static async Task SeedAsync(StoreContext context)
    {

        if (!context.Products.Any())
        {
            var productsJson = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
            if (products != null && products.Any())
            {
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
