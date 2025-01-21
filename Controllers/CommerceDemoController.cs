using CommerceApiDem.Data;
using CommerceApiDem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommerceDemoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public CommerceDemoController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<IEnumerable<CommerceUser>>> Users()
        {
            if (_context == null || _context.Users == null)
                return NotFound();

            var query = from u in _context.Users 
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        orderby u.UserName select new CommerceUser(u, r.Name == "ADMIN");

            return await query.ToListAsync();
        }

        
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(string userName)
        {
            Debug.WriteLine($"SetUser {userName}");


            if (string.IsNullOrEmpty(userName))
                return BadRequest();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
                return Unauthorized();

            var isAdmin = await (from ur in _context.UserRoles
                                 join r in _context.Roles on ur.RoleId equals r.Id
                                 where ur.UserId == user.Id && r.Name == "ADMIN"
                                 select ur).AnyAsync();

            var commerceUser = new CommerceUser(user, isAdmin);

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var secretKey = _config["Jwt:Key"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, userName) // Adding username as a claim
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);


            var tokenHandler = new JwtSecurityTokenHandler();            
            var tokenString = tokenHandler.WriteToken(token);

            

            Debug.WriteLine($"SetUser {userName} complete");

            return Ok( new { user = commerceUser, token = tokenString });
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
