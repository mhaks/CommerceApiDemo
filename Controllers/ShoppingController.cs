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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {

        string _userId = string.Empty;

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ShoppingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        private void Initialize()
        {
            //_userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _userId = "4151283c-1311-4340-af4b-7862b384a330";
        }
       

        #region Product

        //[Authorize]
        [HttpGet]
        [Route("Categories")]
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
        [Route("Brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetProductBrands()
        {
            if (_context == null || _context.Product == null)
            {
                return new List<string>();
            }

            return await _context.Product
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => x.Brand)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }   


        [HttpGet]
        [Route("Products/TopSelling/{count}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTopProducts(int count = 4)
        {
            return  await _context.Product
                .OrderByDescending(a => _context.OrderProduct.Count(b => b.ProductId == a.Id))
                .Take(count)
                .ToListAsync();
        }

        [HttpGet]
        [Route("Products/Search/")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? searchString = null, int? categoryId = null)
        {
            Console.WriteLine($"Search: {searchString} - {categoryId}");
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
        [Route("Products/{id}")]
        public async Task<ActionResult<Product>> GetProducts(int id)
        {
            if ( _context == null || _context.Product == null)
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
        [Route("Cart/Count")]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            if (_context == null || _context.Order == null)
            {
                return NotFound();
            }

            Initialize();
            Debug.WriteLine($"GetCartItemCount: {_userId}");

            var order = await _context.Order
                   .AsNoTracking()
                   .Where(o => o.UserId == _userId)
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


        [HttpGet]
        [Route("Cart")]
        public async Task<ActionResult<Order>> GetCart()
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            Initialize();
            Debug.WriteLine($"GetCart: {_userId}");

            var query = _context.Order.Where(c => c.UserId == _userId).AsNoTracking();
            var order = await GetCartOrder(query);
            return order;
        }

        [HttpPut]
        [Route("Cart/Add")]
        public async Task<ActionResult<int>> AddCartProduct([FromForm] int productId, [FromForm] int quantity)
        {
            
            if (_context == null || _context.Product == null || _context.Order == null)
                return NotFound();

            Initialize();
            Debug.WriteLine($"AddCartProduct: {_userId}");

            var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                var msg = "Product Quantity to add must be greater than 0";
                var customError = new
                {
                    Message = msg,
                    Errors = new List<string> { msg }// Initialize the Errors list
                };
                return BadRequest();
            }

            var query = _context.Order.Where(c => c.UserId == _userId);
            var order = await GetCartOrder(query);


            var oproduct = order.OrderProducts.FirstOrDefault(op => op.ProductId == productId);
            if (oproduct != null)
                oproduct.Quantity += quantity;
            else
                order.OrderProducts.Add(new OrderProduct { Order = order, ProductId = productId, Quantity = quantity, Price = product.Price });


            if (!order.OrderHistory.Any()) 
            {
                order.OrderHistory.Add(new OrderHistory { Order = order, OrderDate = DateTime.UtcNow, OrderStatusId = (int)OrderState.Cart });                
            };

            if (order.Id == 0 && !String.IsNullOrEmpty(_userId))
            {
                order.UserId = _userId;
                _context.Order.Add(order);
            }

            await _context.SaveChangesAsync();

            return order.Id;

        }

        [HttpPut]
        [Route("Cart/Edit")]
        public async Task<ActionResult<int>> EditCartProduct([FromForm] int orderId, [FromForm] int orderProductId, [FromForm] int quantity, [FromForm] string action)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            Initialize();
            Debug.WriteLine($"EditCart: {_userId}");

            var query = _context.Order.Where(o => o.Id == orderId);
            var order = await GetCartOrder(query);

            var prod = order.OrderProducts.Where(op => op.Id == orderProductId).FirstOrDefault();
            if (prod != null)
            {
                if (action == "update")
                {
                    if (quantity > 0)
                    {
                        prod.Quantity = quantity;
                    }
                    else
                    {
                        order.OrderProducts.Remove(prod);

                    }
                }
                else if (action == "remove")
                {
                    order.OrderProducts.Remove(prod);
                }
            }

            // no products, remove empty order, no
            if (!order.OrderProducts.Any())
            {
                _context.Order.Remove(order);
                
            }

            await _context.SaveChangesAsync();

            order = await GetCartOrder(query);
            return order.Id;
        }

        [HttpPost]
        [Route("Cart/Checkout")]
        public async Task<ActionResult<int>> Checkout([FromForm] int orderId, [FromForm] string cardName, [FromForm] string cardNumber, [FromForm] string cardExpiration, [FromForm] string cardCVV)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            Initialize();
            Debug.WriteLine($"Checkout: {_userId}");

            if (String.IsNullOrEmpty(cardName) || cardName.Length < 3
                || String.IsNullOrEmpty(cardNumber) || cardNumber.Length < 16
                || String.IsNullOrEmpty(cardExpiration) || cardExpiration.Length < 4
                || String.IsNullOrEmpty(cardCVV) || cardCVV.Length < 3)
            {
                var msg = "Credit Card information is required";
                var customError = new
                {
                    Message = msg,
                    Errors = new List<string> { msg }// Initialize the Errors list
                };
                return BadRequest(customError);
            }
         

            var order = await _context.Order
                .Where(o => o.Id == orderId && o.UserId == _userId)
                .Include(p => p.OrderProducts)
                .ThenInclude(p => p.Product)
                .Include(h => h.OrderHistory)
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            var processing = new OrderHistory { OrderId = orderId, OrderDate = DateTime.UtcNow, OrderStatusId = (int)OrderState.Processing };
            order.OrderHistory.Add(processing);

            foreach (var item in order.OrderProducts)
            {
                var product = _context.Product.Where(p => p.Id == item.ProductId).FirstOrDefault();
                if (product == null) continue;
                product.AvailableQty = product.AvailableQty - item.Quantity;
            }

            await _context.SaveChangesAsync();

            return orderId;
        }


        async Task<Order> GetCartOrder(IQueryable<Order> query)
        {
            var order = await query
                .Include(c => c.User)
                .ThenInclude(c => c.StateLocation)
                .Include(c => c.OrderProducts)
                .ThenInclude(p => p.Product)
                .Include(c => c.OrderHistory)
                .ThenInclude(c => c.OrderStatus)
                .OrderBy(x => x.Id)
                .LastOrDefaultAsync();

            if (order == null || order.OrderHistory == null || !order.OrderHistory.Any())
            {
                var user = await _context.Users.Where(x => x.Id == _userId).Include(c => c.StateLocation).FirstAsync();
                return new Order { OrderProducts = new List<OrderProduct>(), OrderHistory = new List<OrderHistory>(), UserId = user.Id, User = user};
            }

            var history = order.OrderHistory.OrderBy(x => x.OrderDate).LastOrDefault();
            if (history == null || history.OrderStatusId != (int)OrderState.Cart)
            {
                var user = await _context.Users.Where(x => x.Id == _userId).Include(c => c.StateLocation).FirstAsync();
                return new Order { OrderProducts = new List<OrderProduct>(), OrderHistory = new List<OrderHistory>(), UserId = user.Id, User = user };
            };

            return order;
        }

        #endregion



        #region Ordered

        [HttpGet]
        [Route("Orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            Initialize();
            Debug.WriteLine($"GetOrders: {_userId}");

            var orders = await _context.Order
                            .Where(o => o.UserId == _userId)
                            .Include(c => c.User)
                            .ThenInclude(s => s.StateLocation)
                            .Include(p => p.OrderProducts)
                            .ThenInclude(p => p.Product)
                            .Include(h => h.OrderHistory)
                            .ThenInclude(s => s.OrderStatus)
                            .OrderByDescending(o => o.Id)
                            .AsNoTracking()
                            .ToListAsync();

            var removes = new List<Order>();
            foreach (var order in orders)
            {
                // don't need to show order in cart
                var history = order.OrderHistory.OrderBy(x => x.OrderDate).LastOrDefault();
                if (history == null || history.OrderStatusId == (int)OrderState.Cart)
                {
                    removes.Add(order);
                    break;
                }
            }

            foreach (var item in removes)
                orders.Remove(item);

            return orders;
        }

        [HttpGet]
        [Route("Orders/{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            Initialize();
            Debug.WriteLine($"GetOrder: {_userId}");

            var order = await _context.Order
                        .Where(o => o.Id == id && o.UserId == _userId)
                        .Include(c => c.User)
                        .ThenInclude(s => s.StateLocation)
                        .Include(p => p.OrderProducts)
                        .ThenInclude(p => p.Product)
                        .Include(h => h.OrderHistory)
                        .ThenInclude(s => s.OrderStatus)
                        .OrderByDescending(o => o.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            // don't need to show initial cart
            order.OrderHistory.Remove(order.OrderHistory.First(h => h.OrderStatusId == (int)OrderState.Cart));

            return Ok(order);
        }
        
        [HttpGet]
        [Route("OrderStatuses")]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStatuses()
        {
            if (_context == null || _context.OrderStatus == null)
                return NotFound();

            return await _context.OrderStatus
                .AsNoTracking()
                .Where(x => x.Id != (int)OrderState.Cart)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }   

        #endregion

    }
}
