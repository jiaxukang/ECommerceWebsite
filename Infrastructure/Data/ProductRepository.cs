using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext storeContext) : IProductRepository
{
    public void AddProduct(Product product)
    {
        storeContext.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {

        storeContext.Products.Remove(product);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await storeContext.Products
            .Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await storeContext.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? type, string? brand, string? sort)
    {
        var query = storeContext.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(p => p.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(p => p.Brand == brand);
        }

        query = sort.ToLower() switch
        {
            "priceasc" => query.OrderBy(p => p.Price),
            "pricedesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await storeContext.Products
            .Select(p => p.Type)
            .Distinct()
            .ToListAsync();
    }

    public bool ProductExists(int id)
    {
        return storeContext.Products.Any(p => p.Id == id);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await storeContext.SaveChangesAsync() > 0;
    }

    public void UpdateProduct(Product product)
    {
        storeContext.Products.Update(product);
    }
}
