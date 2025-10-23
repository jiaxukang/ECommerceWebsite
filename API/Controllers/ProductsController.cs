using System;
using API.RequestHelper;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specification;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(IUnitOfWork unitOfWork) : BaseController
{
    [Cached(600)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
        [FromQuery] SpecProductParas paras)
    {
        var spec = new ProductSpecification(paras);

        return await CreatePageResult(unitOfWork.Repository<Product>(), spec, paras.PageIndex, paras.PageSize);
    }

    [Cached(600)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    [Cached(600)]
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await unitOfWork.Repository<Product>().ListAsync(spec));
    }

    [Cached(600)]
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await unitOfWork.Repository<Product>().ListAsync(spec));
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        Console.WriteLine("Creating product...");
        unitOfWork.Repository<Product>().Add(product);
        if (await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        return BadRequest("Failed to create product");
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id || !ProductExists(id))
        {
            return BadRequest();
        }

        unitOfWork.Repository<Product>().Update(product);

        if (await unitOfWork.Complete())
        {
            return NoContent();
        }

        return BadRequest("Failed to update product");
    }

    [InvalidateCache("api/products|")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        unitOfWork.Repository<Product>().Delete(product);
        if (await unitOfWork.Complete())
        {
            return NoContent();
        }

        return BadRequest("Failed to delete product");
    }


    private bool ProductExists(int id)
    {
        return unitOfWork.Repository<Product>().EntityExists(id);
    }

}
