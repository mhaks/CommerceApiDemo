using CommerceApiDem.Data;
using CommerceApiDem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                            .Include(o => o.OrderHistory)
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

    }

}
