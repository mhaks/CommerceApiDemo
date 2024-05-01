using CommerceApiDem.Data;
using CommerceApiDem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
        public async Task<ActionResult<IEnumerable<CommerceUser>>> Users()
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            var query = from u in _context.Users orderby u.UserName select new CommerceUser(u, null, null);

            return await query.ToListAsync();
        }


        [HttpGet]
        [Route("Users/{id}")]
        public async Task<ActionResult<CommerceUser>> GetUser(string id)
        {
            ApplicationUser? appUser = null;
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                Debug.WriteLine("GetUser: "  + User.Identity == null ? "No user" : "User not authenticated");
                // Find the default user by username                
                appUser = await _userManager.FindByNameAsync("jerry");
                if (appUser != null)
                {
                    // Sign in the default user
                    await _signInManager.SignInAsync(appUser, isPersistent: false);
                    Debug.WriteLine($"GetUser {appUser.UserName} logged in");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                Debug.WriteLine("GetUser authenticated: " + User.Identity.Name);
                appUser = await _userManager.FindByNameAsync(User.Identity?.Name ?? "jerry");
            }
            
        
            if (appUser == null)
                return NotFound();

            return new CommerceUser(appUser, User.Identity.IsAuthenticated, User.IsInRole("ADMIN"));
        }

        [HttpPut]
        [Route("Users")]
        public async Task<ActionResult<CommerceUser>> SetUser([FromForm] string userName)
        {
            Debug.WriteLine($"SetUser {userName} logging in");


            if (string.IsNullOrEmpty(userName))
                return BadRequest();
                        
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound();

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(appUser, isPersistent: false);
            Debug.WriteLine($"SetUser {userName} logged in");

            return new CommerceUser(appUser, User.Identity.IsAuthenticated, User.IsInRole("ADMIN"));
        }


        public class CommerceUser
        {
            public CommerceUser(ApplicationUser user, bool? isAuthenticated, bool? isAdministrator)
            {
                FirstName = user.FirstName;
                LastName = user.LastName;
                Id = user.Id;
                UserName = user.UserName;
                IsAuthenticated = isAuthenticated;
                IsAdministrator = isAdministrator;
            }

            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public string? UserName { get; set; } = string.Empty;
            public bool? IsAuthenticated { get; set; }
            public bool? IsAdministrator { get; set; }
        }

    }

}
