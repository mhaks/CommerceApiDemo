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

            var query = from u in _context.Users orderby u.UserName select new CommerceUser(u, u.UserName == "administrator");

            return await query.ToListAsync();
        }

        
        [HttpPost]
        [Route("Users")]
        public async Task<ActionResult<CommerceUser>> SetUser(string userName)
        {
            Debug.WriteLine($"SetUser {userName}");


            if (string.IsNullOrEmpty(userName))
                return BadRequest();
                        
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser == null)
                return NotFound();

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(appUser, isPersistent: false);
            Debug.WriteLine($"SetUser {userName} complete");

            return new CommerceUser(
                                    appUser,
                                    User.Identity != null && User.IsInRole("ADMIN")
                                );

        }

        

        public class CommerceUser
        {
            public CommerceUser(ApplicationUser user, bool isAdministrator)
            {
                FirstName = user.FirstName;
                LastName = user.LastName;               
                UserName = user.UserName;
                IsAdministrator = isAdministrator;
            }

            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? UserName { get; set; } = string.Empty;
            public bool IsAdministrator { get; set; }
        }


        
        

    }

}
