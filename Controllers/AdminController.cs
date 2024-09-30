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
        [Route("Summary/Sales")]
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
        [Route("Summary/Orders")]
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
        [Route("Summary/Inventory")]
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
        [Route("Orders/States")]
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
        public async Task<ActionResult<IEnumerable<OrdersResponse>>> GetOrders(int? orderId, string? userName, int? statusId)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            var query = _context.Order.Select(o => o);
           
            if (orderId.HasValue)
                query = query.Where(o => o.Id == orderId);

            if (!string.IsNullOrEmpty(userName))
                query = query.Where(o => o.User.UserName == userName);

            if (statusId.HasValue)
                query = query.Where(o => o.OrderHistory.OrderByDescending(h => h.OrderDate).First().OrderStatusId == statusId);

            var orders  = await query
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
                    UserName = order.User?.UserName ?? string.Empty,
                    FirstName = order.User?.FirstName ?? string.Empty,
                    LastName = order.User?.LastName ?? string.Empty,
                    Email = order.User?.Email ?? string.Empty,
                    PhoneNumber = order.User?.PhoneNumber ?? string.Empty,
                    Address1 = order.User?.Address1 ?? string.Empty,
                    Address2 = order.User?.Address2 ?? string.Empty,
                    City = order.User?.City ?? string.Empty,
                    StateLocationId = order.User?.StateLocationId ?? 1,
                    StateLocation = order.User?.StateLocation.Abbreviation ?? string.Empty,
                    PostalCode = order.User?.PostalCode ?? string.Empty
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

        [HttpPut]
        [Route("Orders/{orderId}/State/{stateId}")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderState(int orderId, int stateId)
        {
            if (_context == null || _context.Order == null)
                return NotFound();

            var order = await _context.Order
                .Include(o => o.OrderHistory)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            var newState = await _context.OrderStatus.FindAsync(stateId);

            if (newState == null)
                return NotFound();

            var newHistory = new OrderHistory
            {
                OrderId = order.Id,
                OrderStatusId = stateId,
                OrderDate = DateTime.UtcNow  
            };

            _context.Add(newHistory);
            await _context.SaveChangesAsync();

            return await GetOrder(orderId);
        }


        [HttpGet]
        [Route("Products")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts(string? search, string? brand, int? categoryId, bool? isActive)
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            var query = _context.Product.Select(p => p);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search));

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(p => p.Brand == brand);

            if (categoryId.HasValue)
                query = query.Where(p => p.ProductCategoryId == categoryId);

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive);

            var products = await query
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

        [HttpPut]
        [Route("Products")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct([FromForm] ProductRequest product)
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            //validate user fields
            var msgs = new List<string>();

            if (product.Title.IsNullOrEmpty())
                msgs.Add("Product title is required.");
                
            if (product.CategoryId == 0)
                msgs.Add("Category is required.");               

            if (product.Price <= 0)
                msgs.Add("Price must be greater than zero.");
            
            if (product.AvailableQty < 0)
                msgs.Add("Available quantity must be zero or greater.");
                
            if (msgs.Count > 0)
                msgs.Add("All fields are required.");

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


            if (!product.Id.HasValue)
            {
                var newProduct = new Product
                {
                    Title = product.Title,
                    Description = product.Description,
                    Brand = product.Brand,
                    ModelNumber = product.Model,
                    ProductCategoryId = product.CategoryId,
                    Price = product.Price,
                    AvailableQty = product.AvailableQty,
                    IsActive = product.IsActive
                };

                _context.Add(newProduct);
                await _context.SaveChangesAsync();
                product.Id = newProduct.Id;
            }
            else
            {
                var existingProduct = await _context.Product.FindAsync(product.Id);

                if (existingProduct == null)
                    return NotFound();

                existingProduct.Title = product.Title;
                existingProduct.Description = product.Description;
                existingProduct.Brand = product.Brand;
                existingProduct.ModelNumber = product.Model;
                existingProduct.ProductCategoryId = product.CategoryId;
                existingProduct.Price = product.Price;
                existingProduct.AvailableQty = product.AvailableQty;
                existingProduct.IsActive = product.IsActive;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
            }

            return new ProductResponse
            {
                Id = product.Id.Value,
                Title = product.Title,
                Description = product.Description,
                Brand = product.Brand,
                Model = product.Model,
                CategoryId = product.CategoryId,
                Category = _context.ProductCategory.Find(product.CategoryId)?.Title ?? string.Empty,
                Price = product.Price,
                AvailableQty = product.AvailableQty,
                IsActive = product.IsActive
            };
        }


        [HttpGet]
        [Route("Products/Categories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetCategories(string? search)
        {
            if (_context == null || _context.ProductCategory == null)
                return NotFound();

            var query = _context.ProductCategory.Select(c => c);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Title.Contains(search));

            return await query
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        [HttpGet]
        [Route("Products/Categories/{id}")]
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
        [Route("Products/Categories")]
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
        [Route("Products/Brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetBrands()
        {
            if (_context == null || _context.Product == null)
                return NotFound();

            return await _context.Product
                .AsNoTracking()
                .Select(x => x.Brand)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }


        [HttpGet]
        [Route("Customers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetCustomers(string? search)
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            var customers = await _userManager.GetUsersInRoleAsync("CUSTOMER");

            var query = from c in customers
                        join st in _context.StateLocation on c.StateLocationId equals st.Id
                        select c;

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => (c.UserName != null && c.UserName.Contains(search)) || c.FirstName.Contains(search) || c.LastName.Contains(search));

            
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
        public async Task<ActionResult<CustomerResponse>> UpdateCustomer([FromForm] CustomerRequest customer)
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            if (customer == null)
            {
                var msg = "Customer data is required.";
                var customError = new
                {
                    Message = msg,
                    Errors = new List<string> { msg }
                };
                return BadRequest(customError);
            }

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

            var stateLocation = await _context.StateLocation.FindAsync(customer.StateLocationId);
            if (customer.StateLocationId == 0 || stateLocation == null)   
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


            ApplicationUser user;
            if (string.IsNullOrEmpty(customer.Id))
            {
                if (!string.IsNullOrEmpty(customer.UserName))
                    customer.UserName = customer.UserName.ToLower();
                

                if (await _context.Users.AnyAsync(u => u.UserName == customer.UserName))
                {
                    var msg = "Username already exists.";
                    var customError = new
                    {
                        Message = msg,
                        Errors = new List<string> { msg }// Initialize the Errors list
                    };
                    return BadRequest(customError);
                }
                                
                user = new ApplicationUser
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
            }
            else
            {
               if (_userManager == null)
                    return BadRequest();

                user = await _userManager.FindByIdAsync(customer.Id);

                if (user == null)
                {
                    var msg = "User not found.";
                    var customError = new
                    {
                        Message = msg,
                        Errors = new List<string> { msg }
                    };
                    return BadRequest(customError);
                }

                customer.UserName = customer.UserName.ToLower();
                if (user.UserName != customer.UserName && await _context.Users.AnyAsync(u => u.UserName == customer.UserName))
                {
                    var msg = "Username already exists.";
                    var customError = new
                    {
                        Message = msg,
                        Errors = new List<string> { msg }// Initialize the Errors list
                    };
                    return BadRequest(customError);
                }

                user.UserName = customer.UserName;
                user.FirstName = customer.FirstName;
                user.LastName = customer.LastName;
                user.Email = customer.Email;
                user.NormalizedEmail = customer.Email.ToUpper();
                user.PhoneNumber = customer.PhoneNumber;
                user.Address1 = customer.Address1;
                user.Address2 = customer.Address2;
                user.City = customer.City;
                user.StateLocationId = customer.StateLocationId;
                user.PostalCode = customer.PostalCode;

                await _userManager.UpdateAsync(user);
                
            }

            return new CustomerResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address1 = user.Address1,
                Address2 = user.Address2 ?? String.Empty,
                City = user.City,
                StateLocationId = user.StateLocationId,
                StateLocation = stateLocation?.Abbreviation ?? string.Empty,
                PostalCode = user.PostalCode
            };
        }

        [HttpGet]
        [Route("UnitedStates")]
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
