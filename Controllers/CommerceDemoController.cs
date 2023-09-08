using CommerceApiDem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommerceDemoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommerceDemoController(ApplicationDbContext context)
        {
            _context = context;
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
    
        public class AppUser
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public string? UserName { get; set; } = string.Empty;
        }

    }

}
