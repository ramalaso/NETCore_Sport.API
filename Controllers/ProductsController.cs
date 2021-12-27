using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPlusSport.API.Classes;
using HPlusSport.API.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("v{a:apiVersion}/[controller]")]
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

        [HttpPost]
        public async Task<ActionResult<Product>> PostProducts([FromBody]Product product) {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                "GetProduct",
                new {id = product.Id},
                product
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct( int id, [FromBody] Product product) {
            if(id!=product.Id) {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(_context.Products.Find(id)==null) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id) {
            var product = await _context.Products.FindAsync(id);
            if(product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteMultiple([FromQuery] int[] ids) {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _context.Products.FindAsync(id);
                if(product == null) return NotFound();
                products.Add(product);
            }
             _context.Products.RemoveRange(products);
             await _context.SaveChangesAsync();
            return Ok(products);
        }

    }
}
