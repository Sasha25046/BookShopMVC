using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShopDomain.Model;
using BookShopInfrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopInfrastructure.Controllers
{
    public class GenresController : Controller
    {
        private readonly BookShopContext _context;

        public GenresController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Genres/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Genre genre)
        {
            if (_context.Genres.Any(g => g.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "Такий жанр вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Genre genre)
        {
            if (id != genre.Id) return NotFound();

            if (_context.Genres.Any(g => g.Name == genre.Name && g.Id != genre.Id))
            {
                ModelState.AddModelError("Name", "Такий жанр вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                                       .Include(g => g.Products)
                                       .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null) return NotFound();

            if (genre.Products.Any())
            {
                ViewBag.ErrorMessage = "Неможливо видалити жанр, оскільки в ньому є книги.";
                return View("DeleteError", genre);
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres
                                       .Include(g => g.Products)
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (genre == null) return NotFound();

            if (genre.Products.Any())
            {
                TempData["ErrorMessage"] = "Неможливо видалити жанр, оскільки в ньому є книги.";
                return RedirectToAction(nameof(Index));
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Жанр успішно видалений.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                                       .Include(g => g.Products)
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (genre == null) return NotFound();

            return View(genre);
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}
