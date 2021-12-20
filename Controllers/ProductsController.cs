using System.Linq;
using System.Threading.Tasks;
using HPlusSport.API.Classes;
using HPlusSport.API.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters) {
            IQueryable<Product> products = _context.Products;
            
            if(queryParameters.MinPrice != null && queryParameters.MaxPrice != null) {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value && 
                    p.Price <= queryParameters.MaxPrice.Value
                );
            }

            if(!string.IsNullOrEmpty(queryParameters.Sku)) {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }

            if(!string.IsNullOrEmpty(queryParameters.Name)) {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }

            products = products
                            .Skip(queryParameters.Size * (queryParameters.Page -1))
                            .Take(queryParameters.Size);
            return Ok(await products.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id) {
            var product = await _context.Products.FindAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
    }
}
