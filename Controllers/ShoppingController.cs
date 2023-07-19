using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommerceApiDem.Models;
using CommerceApiDem.Data;

namespace CommerceApiDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShoppingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<ProductCategory> Get()
        {
            if (_context == null || _context.ProductCategory == null)
            {
                return new List<ProductCategory>();
            }

            return _context.ProductCategory
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToList();
        }

        /*
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> Get()
        {
            if(_context == null || _context.ProductCategory == null) 
            {
                return NotFound();
            }

            return await _context.ProductCategory
                .AsNoTracking()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }
        */
    }
}
