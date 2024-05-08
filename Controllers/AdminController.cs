using CommerceApiDem.Data;
using CommerceApiDem.Models;
using CommerceApiDemo.AdminDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public async Task<ActionResult<SalesSummaryResponse>> SalesSummary(int Days)
        {
            if (_context == null || _context.Order == null)
            {
                return NotFound();
            }

            var query = from o in _context.Order select o; ;

            var lookbackDate = DateTime.Now.AddDays(-Days).Date;
            query = query.Where(o => o.OrderHistory.Any(h => h.OrderStatusId == (int)CommerceApiDem.Models.OrderState.Processing && h.OrderDate >= lookbackDate));

            var orders = await query
                            .Include(o => o.OrderProducts)
                            .ThenInclude(p => p.Product)
                            .Include(o => o.OrderHistory)
                            .ThenInclude(h => h.OrderStatus)
                            .Include(c => c.User)
                            .ThenInclude(c => c.StateLocation)
                            .AsNoTracking()
                            .ToListAsync();

            var result = new SalesSummaryResponse
            {
                OrderCount = orders.Count,
                ProductCount = orders.Sum(o => o.OrderProducts.Count),
                Revenue = orders.Sum(o => o.OrderProducts.Sum(p => p.Price * p.Quantity))
            };

            return Ok(result);

        }


        [HttpGet]
        [Route("OrderSummary")]
        public async Task<ActionResult<OrderSummaryResponse>> OrderSummary(int? Days, int? Status)
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



            var result = new OrderSummaryResponse
            { 
                OrderCount = orders.Count, 
                ProductCount = orders.Sum(o => o.OrderProducts.Count()),
                Revenue = orders.Sum(o => o.Subtotal)
            };
            return Ok(result); 
                                       

        }

        [HttpGet]
        [Route("InventorySummary")]
        public async Task<ActionResult<InventorySummaryResponse>> InventorySummary(int threshold)
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

            var result = new InventorySummaryResponse
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
        public async Task<ActionResult<IEnumerable<AdminDto.OrderState>>> GetOrderStates()
        {
            if (_context == null || _context.OrderStatus == null)
                return NotFound();

            return await _context.OrderStatus
                .AsNoTracking()                
                .OrderBy(x => x.Name)
                .Select(x => new AdminDto.OrderState { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        [HttpGet]
        [Route("Orders")]
        public async Task<ActionResult<IEnumerable<OrdersResponse>>> GetOrders()
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            var orders = await _context.Order
                .Include(o => o.OrderProducts)
                .Include(o => o.OrderHistory)
                .Include(c => c.User)
                .ThenInclude(c => c.StateLocation)
                .AsNoTracking()
                .Select(o => new OrdersResponse
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
        [Route("Orders/{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            var order = await _context.Order
                .Include(o => o.OrderProducts)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.ProductCategory)
                .Include(o => o.OrderHistory)
                .ThenInclude(h => h.OrderStatus)
                .Include(c => c.User)
                .ThenInclude(c => c.StateLocation)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var result = new OrderResponse
            {
                Id = order.Id,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                TotalPrice = order.TotalPrice,
                Customer = new CustomerResponse
                {
                    Id = order.User.Id,
                    UserName = order.User.UserName,
                    FirstName = order.User.FirstName,
                    LastName = order.User.LastName,
                    Email = order.User.Email,
                    PhoneNumber = order.User.PhoneNumber,
                    Address1 = order.User.Address1,
                    Address2 = order.User.Address2,
                    City = order.User.City,
                    StateLocationId = order.User.StateLocationId,
                    StateLocation = order.User.StateLocation.Abbreviation,
                    PostalCode = order.User.PostalCode
                },
                Products = order.OrderProducts.Select(p => new ProductResponse
                {
                    Id = p.Product.Id,
                    Title = p.Product.Title,
                    Description = p.Product.Description,
                    Brand = p.Product.Brand,
                    Model = p.Product.ModelNumber,
                    Category = p.Product.ProductCategory.Title,
                }).ToList<ProductResponse>(),
                History = order.OrderHistory.Select(h => new OrderHistoryResponse
                {
                    Id = h.Id,
                    OrderDate = h.OrderDate,
                    OrderStatusId = h.OrderStatusId,
                    OrderStatus = h.OrderStatus.Name                    
                }).ToList<OrderHistoryResponse>(),
            };

            return Ok(result);
        }


        [HttpGet]
        [Route("Products")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            var products = await _context.Product
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .Select(p => new ProductResponse
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
        [Route("Products/{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            var product = await _context.Product
                .Where(p => p.Id == id)
                .Include(p => p.ProductCategory)
                .AsNoTracking()
                .Select(p => new ProductResponse
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
        [Route("Categories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetCategories()
        {
            if (_context == null || _context.ProductCategory == null)
                return NotFound();

            return await _context.ProductCategory
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        [HttpGet]
        [Route("Categories/{id}")]
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
        [Route("Categories")]
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
                return BadRequest(customError);
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
        [Route("Customers/{id}")]
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

        [HttpPut]
        [Route("Customers")]
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer([FromForm] CustomerResponse customer)
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            //validate user fields
            var msgs = new List<string>();
            if (customer.UserName.IsNullOrEmpty())
                msgs.Add("Username is required.");  

            if (customer.FirstName.IsNullOrEmpty())
                msgs.Add("First Name is required.");

            if (customer.LastName.IsNullOrEmpty())
                msgs.Add("Last Name is required.");

            if (customer.Email.IsNullOrEmpty())
                msgs.Add("Email is required.");

            if (customer.PhoneNumber.IsNullOrEmpty())
                msgs.Add("Phone Number is required.");

            if (customer.Address1.IsNullOrEmpty())
                msgs.Add("Address is required.");

            if (customer.City.IsNullOrEmpty())
                msgs.Add("City is required.");

            if (customer.StateLocationId == 0)   
                msgs.Add("State is required.");

            if (customer.PostalCode.IsNullOrEmpty())
                msgs.Add("Postal Code is required.");

            if (msgs.Count > 0)                
            {
                var msg = "All fields are required.";
                msgs.Insert(0, msg);
                var customError = new
                {
                    Message = msg,
                    Errors = msgs // Initialize the Errors list
                };
                return BadRequest(customError);
            }


            if (customer.Id.IsNullOrEmpty())
            {
                customer.UserName = customer.UserName.ToLower();

                if (await _context.Users.AnyAsync(u => u.UserName == customer.UserName))
                {
                    var msg = "Username already exists.";
                    var customError = new
                    {
                        Message = msg,
                        Errors = new List<string> { msg }// Initialize the Errors list
                    };
                    return BadRequest();
                }
                                
                var user = new ApplicationUser
                {
                    UserName = customer.UserName,
                    NormalizedUserName = customer.UserName.ToUpper(),
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    NormalizedEmail = customer.Email.ToUpper(),
                    PhoneNumber = customer.PhoneNumber,
                    Address1 = customer.Address1,
                    Address2 = customer.Address2,
                    City = customer.City,
                    StateLocationId = customer.StateLocationId,
                    PostalCode = customer.PostalCode,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                };

                var hasher = new PasswordHasher<IdentityUser>();
                user.PasswordHash = hasher.HashPassword(user, "password");


                UserStore<IdentityUser> userStore = new UserStore<IdentityUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "CUSTOMER");
                await userStore.Context.SaveChangesAsync();
                customer.Id = user.Id;
                return customer;
            }
            else
            {
                var existingUser = await _userManager.FindByIdAsync(customer.Id);

                if (existingUser == null)
                {
                    var msg = "User not found.";
                    var customError = new
                    {
                        Message = msg,
                        Errors = new List<string> { msg }// Initialize the Errors list
                    };
                    return BadRequest();
                }

                customer.UserName = customer.UserName.ToLower();
                if (existingUser.UserName != customer.UserName && await _context.Users.AnyAsync(u => u.UserName == customer.UserName))
                {
                    var msg = "Username already exists.";
                    var customError = new
                    {
                        Message = msg,
                        Errors = new List<string> { msg }// Initialize the Errors list
                    };
                    return BadRequest(customError);
                }                

                existingUser.UserName = customer.UserName;
                existingUser.FirstName = customer.FirstName;
                existingUser.LastName = customer.LastName;
                existingUser.Email = customer.Email;
                existingUser.NormalizedEmail = customer.Email.ToUpper();
                existingUser.PhoneNumber = customer.PhoneNumber;
                existingUser.Address1 = customer.Address1;
                existingUser.Address2 = customer.Address2;
                existingUser.City = customer.City;
                existingUser.StateLocationId = customer.StateLocationId;
                existingUser.PostalCode = customer.PostalCode;

                await _userManager.UpdateAsync(existingUser);
                return customer;
            }
        }

        [HttpGet]
        [Route("StateLocations")]
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
