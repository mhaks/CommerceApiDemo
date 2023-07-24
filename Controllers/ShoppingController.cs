using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommerceApiDem.Models;
using CommerceApiDem.Data;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShoppingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("ProductCategories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            if (_context == null || _context.ProductCategory == null)
            {
                return new List<ProductCategory>();
            }

            return await _context.ProductCategory
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }


        [HttpGet]
        [Route("TopProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTopProducts()
        {
            return  await _context.Product
                .OrderByDescending(a => _context.OrderProduct.Count(b => b.ProductId == a.Id))
                .Take(4)
                .ToListAsync();
        }

        [HttpGet]
        [Route("Product")]
        public async Task<ActionResult<Product>> GetProduct(int? id)
        {
            if (id == null || _context == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
               .AsNoTracking()
               .Include(x => x.ProductCategory)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                return product;
            }

        }
    }
}
