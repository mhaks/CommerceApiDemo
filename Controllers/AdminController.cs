using CommerceApiDem.Data;
using CommerceApiDem.Models;
using CommerceApiDemo.DtoModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpGet]
        [Route("SalesSummary")]
        public async Task<ActionResult> SalesSummary(int Days)
        {
            if (_context == null || _context.Order == null)
            {
                return NotFound();
            }

            var query = from o in _context.Order select o; ;

            var lookbackDate = DateTime.Now.AddDays(-Days).Date;
            query = query.Where(o => o.OrderHistory.Any(h => h.OrderStatusId == (int)OrderState.Processing && h.OrderDate >= lookbackDate));

            var orders = await query
                            .Include(o => o.OrderProducts)
                            .ThenInclude(p => p.Product)
                            .Include(o => o.OrderHistory)
                            .ThenInclude(h => h.OrderStatus)
                            .Include(c => c.User)
                            .ThenInclude(c => c.StateLocation)
                            .AsNoTracking()
                            .ToListAsync();

            var result = new
            {
                OrderCount = orders.Count,
                ProductCount = orders.Sum(o => o.OrderProducts.Count),
                Revenue = orders.Sum(o => o.OrderProducts.Sum(p => p.Price * p.Quantity))
            };

            return Ok(result);

        }


        [HttpGet]
        [Route("OrderSummary")]
        public async Task<ActionResult> OrderSummary(int? Days, int? Status)
        {
            if (_context == null || _context.Order == null)
            {
                return NotFound();
            }

            var query = from o in _context.Order select o;                            ;


            if (Days.HasValue)
            {
                var lookbackDate = DateTime.Now.AddDays(-Days.Value).Date;
                query = query.Where(o => o.OrderHistory.Any(h => h.OrderDate >= lookbackDate));
            }

            if (Status.HasValue)
            {
                query = query.Where(o => o.OrderHistory.OrderByDescending(h => h.OrderDate).First().OrderStatusId == Status.Value);
            }

            var orders = await query
                            .Include(o => o.OrderProducts)
                            .Include(o => o.OrderHistory)
                            .Include(c => c.User)
                            .ThenInclude(c => c.StateLocation)
                            .AsNoTracking()
                            .ToListAsync();



            var result = new  { 
                OrderCount = orders.Count, 
                ProductCount = orders.Sum(o => o.OrderProducts.Count()),
                Revenue = orders.Sum(o => o.Subtotal)
            };
            return Ok(result); 
                                       

        }

        [HttpGet]
        [Route("InventorySummary")]
        public async Task<ActionResult> InventorySummary(int threshold)
        {
            if (_context == null || _context.Product == null)
            {
                return NotFound();
            }

            var query = from p in _context.Product select p;

            var products = await query
                            .Include(p => p.ProductCategory)                            
                            .AsNoTracking()
                            .ToListAsync();

            var result = new
            {
                Active = products.Count(p => p.IsActive),
                Inactive = products.Count(p => !p.IsActive),
                AboveThreshold = products.Count(p => p.IsActive && p.AvailableQty > threshold),
                BelowThreshold = products.Count(p => p.IsActive && p.AvailableQty <= threshold && p.AvailableQty > 0),
                OutOfStock = products.Count(p => p.IsActive && p.AvailableQty == 0)
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("OrderStates")]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetOrderStates()
        {
            if (_context == null || _context.OrderStatus == null)
                return NotFound();

            return await _context.OrderStatus
                .AsNoTracking()                
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        [HttpGet]
        [Route("Orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            var orders = await _context.Order
                .Include(o => o.OrderProducts)
                .Include(o => o.OrderHistory)
                .Include(c => c.User)
                .ThenInclude(c => c.StateLocation)
                .AsNoTracking()
                .Select(o => new
                {
                    OrderId = o.Id,
                    UserName = o.User.UserName,
                    OrderDate = o.OrderHistory
                        .Where(h => h.OrderStatusId == 2)
                        .OrderBy(h => h.OrderDate)
                        .Select(h => h.OrderDate)
                        .FirstOrDefault(),
                    StatusId = o.OrderHistory
                        .OrderBy(h => h.OrderDate)
                        .Select(h => h.OrderStatusId)
                        .LastOrDefault(),
                    StatusName = o.OrderHistory
                        .OrderBy(h => h.OrderDate)
                        .Select(h => h.OrderStatus.Name)
                        .LastOrDefault(),
                    StatusDate = o.OrderHistory
                        .OrderBy(h => h.OrderDate)
                        .Select(h => h.OrderDate)
                        .LastOrDefault(),
                })
                .OrderBy(o => o.OrderId)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet]
        [Route("Order/{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            var order = await _context.Order
                .Include(o => o.OrderProducts)
                .ThenInclude(p => p.Product)
                .Include(o => o.OrderHistory)
                .ThenInclude(h => h.OrderStatus)
                .Include(c => c.User)
                .ThenInclude(c => c.StateLocation)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return order;
        }


        [HttpGet]
        [Route("Products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            var products = await _context.Product
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Brand = p.Brand,
                    Model = p.ModelNumber,
                    Category = p.ProductCategory.Title,
                    CategoryId = p.ProductCategory.Id,
                    Price = p.Price,
                    AvailableQty = p.AvailableQty,
                    IsActive = p.IsActive
                })
                .OrderBy(p => p.Title)
                .ToListAsync();

            return Ok(products);
        }


        [HttpGet]
        [Route("Product/{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            var product = await _context.Product
                .Where(p => p.Id == id)
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Brand = p.Brand,
                    Model = p.ModelNumber,
                    Category = p.ProductCategory.Title,
                    CategoryId = p.ProductCategory.Id, 
                    Price = p.Price,
                    AvailableQty = p.AvailableQty,
                    IsActive = p.IsActive
                })
                .FirstOrDefaultAsync();

            if (product == null)    
                return NotFound();
            else
                return product;
        }


        [HttpGet]
        [Route("Category/{id}")]
        public async Task<ActionResult<ProductCategory>> GetCategory(int id)
        {
            if (_context == null || _context.ProductCategory == null)
                return NotFound();

            var category = await _context.ProductCategory
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();
            else
                return category;
        }

        [HttpPut]
        [Route("Category")]
        public async Task<ActionResult<ProductCategory>> UpdateCategory([FromForm] ProductCategory category)
        {
            if (_context == null || _context.ProductCategory == null)
                return NotFound();

            if (category.Title.IsNullOrEmpty())
            {
                var msg = "Category title is required.";
                var customError = new
                {
                    Message = msg,
                    Errors = new List<string> { msg }// Initialize the Errors list
                };
                return BadRequest();
            }


            if (category.Id == 0)
            {
                _context.Add(category);
            }
            else
            {
                _context.Update(category);
            }

            await _context.SaveChangesAsync();

            return category;
        }
        

        [HttpGet]
        [Route("Customers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetCustomers()
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            var customers = await _userManager.GetUsersInRoleAsync("CUSTOMER");

            var query = from c in customers 
                        join st in _context.StateLocation on c.StateLocationId equals st.Id
                        select c;

            var users = query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
 
            return Ok(users);
        }

        [HttpGet]
        [Route("Customer/{id}")]
        public async Task<ActionResult<ApplicationUser>> GetCustomer(string id)
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            var customers = await _userManager.GetUsersInRoleAsync("CUSTOMER");

            var query = from c in customers
                        where c.Id == id
                        join st in _context.StateLocation on c.StateLocationId equals st.Id
                        select c;

            var user = query.FirstOrDefault();

            if (user == null)
                return NotFound();
            else
                return user;
        }

        [HttpGet]
        [Route("States")]
        public async Task<ActionResult<IEnumerable<StateLocation>>> GetStates()
        {
            if (_context == null || _context.StateLocation == null)
                return NotFound();

            return await _context.StateLocation
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }

}
