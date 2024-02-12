using CommerceApiDem.Data;
using CommerceApiDem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommerceDemoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CommerceDemoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<IEnumerable<AppUser>>> Users()
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            

            var query = from u in _context.Users orderby u.UserName select new AppUser { FirstName = u.FirstName, LastName = u.LastName, Id = u.Id, UserName = u.UserName };

            return await query.ToListAsync();
        }


        [HttpGet]
        [Route("User")]
        public async Task<ActionResult<AppUser>> GetUser()
        {
            ApplicationUser? applicationUser = null;
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                // Find the default user by username                
                applicationUser = await _userManager.FindByNameAsync("jerry");
                if (applicationUser != null)
                {
                    // Sign in the default user
                    await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                applicationUser = await _userManager.FindByNameAsync(User.Identity?.Name ?? "jerry");
            }
            

        
            if (applicationUser == null)
                return NotFound();

            return new AppUser { FirstName = applicationUser.FirstName, LastName = applicationUser.LastName, Id = applicationUser.Id, UserName = applicationUser.UserName };
        }

        [HttpPut]
        [Route("User")]
        public async Task<ActionResult> SetUser([FromForm] string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest();

            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser == null)
                return NotFound();

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(appUser, isPersistent: false);
            Console.WriteLine($"User {userName} logged in");

            return Ok();
        }


        public class AppUser
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public string? UserName { get; set; } = string.Empty;
        }

    }

}
