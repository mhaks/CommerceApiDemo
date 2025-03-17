using CommerceDemo.Data;
using CommerceDemo.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CommerceApiDemo.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {

        private readonly CommerceDemoContext _context;
        

        public ShoppingController(CommerceDemoContext context)
        {
            _context = context;
        }



        #region Product


        [HttpGet]
        [Route("Products/Categories")]
        public async Task<ActionResult<IEnumerable<ShoppingDto.ProductCategory>>> GetProductCategories()
        {
            if (_context == null || _context.ProductCategory == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return await _context.ProductCategory
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .Select(x => new ShoppingDto.ProductCategory { Id = x.Id, Title = x.Title })
                .ToListAsync();
        }

        [HttpGet]
        [Route("Products/Sales/{count}")]
        public async Task<ActionResult<IEnumerable<ShoppingDto.Product>>> GetTopProducts(int count = 4)
        {
            if (_context == null || _context.ProductCategory == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return await _context.Product
                .OrderByDescending(a => _context.OrderProduct.Count(b => b.ProductId == a.Id))
                .Take(count)
                .Select(x => new ShoppingDto.Product { Id = x.Id, Title = x.Title, Description = x.Description, Brand = x.Brand, Price = x.Price, AvailableQty = x.AvailableQty, Quantity = 0, Model = x.ModelNumber, Category = x.ProductCategory != null ? x.ProductCategory.Title : String.Empty })
                .ToListAsync();
        }

        [HttpGet]
        [Route("Products/Search/")]
        public async Task<ActionResult<IEnumerable<ShoppingDto.Product>>> GetProducts(string? searchString = null, int? categoryId = null)
        {
            if (_context == null || _context.ProductCategory == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

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
                   .Select(x => new ShoppingDto.Product { Id = x.Id, Title = x.Title, Description = x.Description, Brand = x.Brand, Price = x.Price, AvailableQty = x.AvailableQty, Quantity = 0, Model = x.ModelNumber, Category = x.ProductCategory != null ? x.ProductCategory.Title : String.Empty })
                   .ToListAsync();
        }

        [HttpGet]
        [Route("Products/{id}")]
        public async Task<ActionResult<ShoppingDto.Product>> GetProducts(int id)
        {
            if (_context == null || _context.ProductCategory == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var product = await _context.Product
               .AsNoTracking()
               .Include(x => x.ProductCategory)
               .Select(x => new ShoppingDto.Product { Id = x.Id, Title = x.Title, Description = x.Description, Brand = x.Brand, Price = x.Price, AvailableQty = x.AvailableQty, Quantity = 0, Model = x.ModelNumber, Category = x.ProductCategory != null ? x.ProductCategory.Title : String.Empty })
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

        [Authorize]
        [HttpGet]
        [Route("Cart")]
        public async Task<ActionResult<ShoppingDto.Cart>> GetCart()
        {
            if (_context == null || _context.Order == null || _context.OrderProduct == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            string userId = await GetUserId();
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();

            var order = await GetCartOrder(userId);
            var customer = new ShoppingDto.Customer { Id = order.User.Id, UserName = order.User.UserName ?? String.Empty, FirstName = order.User.FirstName, LastName = order.User.LastName, Email = order.User.Email ?? String.Empty, PhoneNumber = order.User.PhoneNumber ?? String.Empty, Address1 = order.User.Address1, Address2 = order.User.Address2 ?? String.Empty, City = order.User.City, State = order.User.StateLocation != null ? order.User.StateLocation.Name : String.Empty, PostalCode = order.User.PostalCode, TaxRate = order.User.StateLocation?.TaxRate ?? 0 };
            var products = order.OrderProducts.Select(x => new ShoppingDto.Product { Id = x.ProductId, Title = x.Product.Title, Description = x.Product.Description, Brand = x.Product.Brand, Price = x.Price, AvailableQty = x.Product.AvailableQty, Quantity = x.Quantity, Model = x.Product.ModelNumber, Category = x.Product.ProductCategory != null ? x.Product.ProductCategory.Title : String.Empty }).ToList();
            var cart = new ShoppingDto.Cart {Id = order.Id, Customer = customer, Products = products };
            return cart;
        }


        [Authorize]
        [HttpPost]
        [Route("Cart/Products/")]
        public async Task<ActionResult<ShoppingDto.Cart>> UpdateCartProduct([FromForm] int productId, [FromForm] int quantity)
        {
            if (_context == null || _context.Order == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            string userId = await GetUserId();
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();


            var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
            if (product == null)
                return NotFound();

            if (quantity <= 0)
            {
                var msg = "Product Quantity must be greater than 0";
                var customError = new
                {
                    Message = msg,
                    Errors = new List<string> { msg }// Initialize the Errors list
                };
                return BadRequest();
            }


            var cart = await GetCartOrder(userId);
            var orderProduct = cart.OrderProducts.Where(op => op.ProductId == productId).FirstOrDefault();
            if (orderProduct != null)
            {
                orderProduct.Quantity = quantity;
            }
            else
            {
                cart.OrderProducts.Add(new OrderProduct { ProductId = productId, Quantity = quantity, Price = product.Price });
            }

            if (cart.OrderHistory == null || !cart.OrderHistory.Any())
            {
                cart.OrderHistory = new List<OrderHistory> { new OrderHistory { OrderId = cart.Id, OrderDate = DateTime.UtcNow, OrderStatusId = (int)OrderState.Cart } };
                _context.Order.Add(cart);
            }


            await _context.SaveChangesAsync();

            return await GetCart();
        }


        [Authorize]
        [HttpDelete]
        [Route("Cart/Products/{productId}")]
        public async Task<ActionResult<ShoppingDto.Cart>> RemoveCartProduct(int productId)
        {
            if (_context == null || _context.Order == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            string userId = await GetUserId();
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
            if (product == null)
                return NotFound();


            var order = await GetCartOrder(userId);
            var orderProduct = order.OrderProducts.Where(op => op.ProductId == productId).FirstOrDefault();
            if (orderProduct != null)
                order.OrderProducts.Remove(orderProduct);

            // no products, remove empty order, no
            if (!order.OrderProducts.Any())
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();

            return await GetCart();
        }


        [Authorize]
        [HttpPost]
        [Route("Cart/Checkout")]
        public async Task<ActionResult<int>> Checkout([FromForm] int orderId, [FromForm] string cardName, [FromForm] string cardNumber, [FromForm] string cardExpiration, [FromForm] string cardCVV)
        {
            if (_context == null || _context.Order == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            string userId = await GetUserId();
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();

            Debug.WriteLine($"Checkout: {userId}");

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
                .Where(o => o.Id == orderId && o.User.Id == userId)
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


        async Task<CommerceDemo.Data.Models.Order> GetCartOrder(string userId)
        {
            var order = await _context.Order
                .Where(c => c.User.Id == userId)
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
                var user = await _context.Users.Where(x => x.Id == userId).Include(c => c.StateLocation).FirstAsync();
                return new CommerceDemo.Data.Models.Order { OrderProducts = new List<OrderProduct>(), OrderHistory = new List<OrderHistory>(), UserId = user.Id, User = user };
            }

            var history = order.OrderHistory.OrderBy(x => x.OrderDate).LastOrDefault();
            if (history == null || history.OrderStatusId != (int)OrderState.Cart)
            {
                var user = await _context.Users.Where(x => x.Id == userId).Include(c => c.StateLocation).FirstAsync();
                return new CommerceDemo.Data.Models.Order { OrderProducts = new List<OrderProduct>(), OrderHistory = new List<OrderHistory>(), UserId = user.Id, User = user };
            };

            return order;
        }

        #endregion



        #region Orders

        [Authorize]
        [HttpGet]
        [Route("Orders")]
        public async Task<ActionResult<IEnumerable<ShoppingDto.Order>>> GetOrders()
        {
            if (_context == null || _context.Order == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            string userId = await GetUserId();
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();

            Debug.WriteLine($"GetOrders: {userId}");

            var orders = await _context.Order
                            .Where(o => o.User.Id == userId)
                            .Include(c => c.User)
                            .ThenInclude(s => s.StateLocation)
                            .Include(p => p.OrderProducts)
                            .ThenInclude(p => p.Product)
                            .Include(h => h.OrderHistory)
                            .ThenInclude(s => s.OrderStatus)
                            .OrderByDescending(o => o.Id)
                            .AsNoTracking()
                            .ToListAsync();

            var removes = new List<CommerceDemo.Data.Models.Order>();
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

            var shoppingOrders = new List<ShoppingDto.Order>();
            foreach (var order in orders)
            {
                var customer = new ShoppingDto.Customer { Id = order.User.Id, UserName = order.User.UserName ?? String.Empty, FirstName = order.User.FirstName, LastName = order.User.LastName, Email = order.User.Email ?? String.Empty, PhoneNumber = order.User.PhoneNumber ?? String.Empty, Address1 = order.User.Address1, Address2 = order.User.Address2 ?? String.Empty, City = order.User.City, State = order.User.StateLocation != null ? order.User.StateLocation.Name : String.Empty, PostalCode = order.User.PostalCode, TaxRate = order.User.StateLocation?.TaxRate ?? 0 };
                var products = order.OrderProducts.Select(x => new ShoppingDto.Product { Id = x.ProductId, Title = x.Product.Title, Description = x.Product.Description, Brand = x.Product.Brand, Price = x.Price, AvailableQty = x.Product.AvailableQty, Quantity = x.Quantity, Model = x.Product.ModelNumber, Category = x.Product.ProductCategory != null ? x.Product.ProductCategory.Title : String.Empty }).ToList();
                var history = order.OrderHistory.OrderBy(x => x.OrderDate).Select(x => new ShoppingDto.History { StatusId = x.OrderStatusId, Status = x.OrderStatus.Name, OrderDate = x.OrderDate }).ToList();
                var shoppingOrder = new ShoppingDto.Order { Id = order.Id, Customer = customer, Products = products, History = history };
                shoppingOrders.Add(shoppingOrder);
            }
            return Ok(shoppingOrders);
        }


        [Authorize]
        [HttpGet]
        [Route("Orders/{id}")]
        public async Task<ActionResult<ShoppingDto.Order>> GetOrder(int id)
        {
            if (_context == null || _context.Order == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            string userId = await GetUserId();
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();

            Debug.WriteLine($"GetOrder: {userId}");

            var order = await _context.Order
                        .Where(o => o.Id == id && o.User.Id == userId)
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
        [Route("Orders/States")]
        public async Task<ActionResult<IEnumerable<ShoppingDto.Status>>> GetOrderStatuses()
        {
            if (_context == null || _context.OrderStatus == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return await _context.OrderStatus
                .AsNoTracking()
                .Where(x => x.Id != (int)OrderState.Cart)
                .OrderBy(x => x.Name)
                .Select(x => new ShoppingDto.Status { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        #endregion


        async Task<string> GetUserId()
        {
            Debug.WriteLine("Current User: " + User.Identity?.Name);
            var username = User.Identity?.Name; // Retrieves the username
            if (string.IsNullOrEmpty(username))
                return string.Empty;

            var user = await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
            if (user != null)
            {
                return user.Id;
            }
            else
            {
                return string.Empty;
            }           
        }

    }
}
