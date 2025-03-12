using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly BookShopContext _context;

        public ChartsController(BookShopContext context)
        {
            _context = context;
        }

        [HttpGet("genre-popularity")]
        public async Task<IActionResult> GetGenrePopularity()
        {
            var genreCounts = await _context.Genres
                .Select(g => new
                {
                    Genre = g.Name,
                    Count = g.Products.Count()
                })
                .ToListAsync();

            return Ok(genreCounts);
        }

        [HttpGet("countBooksByYear")]
        public async Task<JsonResult> GetCountBooksByYearAsync(CancellationToken cancellationToken)
        {
            var responseItems = await _context.Products
                .GroupBy(p => p.Year)
                .Select(group => new
                {
                    Year = group.Key,
                    Count = group.Count()
                })
                .OrderBy(item => item.Year)
                .ToListAsync(cancellationToken);

            return new JsonResult(responseItems);
        }

    }
}
