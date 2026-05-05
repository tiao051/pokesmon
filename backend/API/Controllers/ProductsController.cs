using Microsoft.AspNetCore.Mvc;
using backend.Core.Entities;
using backend.Core.DTOs;
using MongoDB.Driver;
using backend.Infrastructure.Persistence;

namespace backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public ProductsController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _context.Products.Find(_ => true).Limit(20).ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (product == null) return NotFound();
        
        return Ok(product);
    }
}
