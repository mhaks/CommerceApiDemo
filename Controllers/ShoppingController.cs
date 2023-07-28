using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommerceApiDem.Models;
using CommerceApiDem.Data;
using System.Security.Claims;

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

        #region Product

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
        [Route("Search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string searchString, int? categoryId)
        {
            if(_context == null || _context.Product == null)
                return NotFound();

            var productsQuery = from p in _context.Product where p.IsActive select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper().Trim();
                productsQuery = productsQuery.Where(p => p.Title.ToUpper().Contains(searchString) || p.Description.ToUpper().Contains(searchString) || p.Brand.ToUpper().Contains(searchString) || p.ProductCategory.Title.ToUpper().Contains(searchString));

            }

            if (categoryId != null)
            {
                productsQuery = productsQuery.Where(c => c.ProductCategoryId == categoryId);                
            }


            return await productsQuery
                   .AsNoTracking()
                   .Include(c => c.ProductCategory)
                   .Take(20)
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

        #endregion


        #region Cart

        [HttpGet]
        [Route("CartCount")]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            if (_context == null || _context.Order == null)
            {
                return NotFound();
            }

            // TODO
            //var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = "4151283c-1311-4340-af4b-7862b384a330";

            var order = await _context.Order
                   .AsNoTracking()
                   .Where(o => o.UserId == userId)
                   .Include(c => c.OrderHistory)
                   .Include(c => c.OrderProducts)
                   .OrderBy(x => x.Id)
                   .LastOrDefaultAsync();

            int itemCount = 0;
            if (order != null && order.OrderHistory != null)
            {
                var history = order.OrderHistory.OrderBy(x => x.OrderDate).LastOrDefault();
                if (history != null && history.OrderStatusId == (int)OrderState.Cart)
                {
                    itemCount = order.OrderProducts.Count;
                }
            }

            return itemCount;
        }

        #endregion
    }
}
