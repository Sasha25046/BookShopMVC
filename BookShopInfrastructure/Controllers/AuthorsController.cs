using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShopDomain.Model;
using BookShopInfrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopInfrastructure.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly BookShopContext _context;

        public AuthorsController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Authors/Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.ToListAsync());
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                                       .Include(a => a.Products)
                                       .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            if (author.Products.Any())
            {
                ViewBag.ErrorMessage = "Неможливо видалити автора, оскільки у нього є книги.";
                return View("DeleteError", author);
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors
                                       .Include(a => a.Products)
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            if (author.Products.Any())
            {
                TempData["ErrorMessage"] = "Неможливо видалити автора, оскільки у нього є книги.";
                return RedirectToAction(nameof(Index));  
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Автор успішно видалений.";
            return RedirectToAction(nameof(Index));  
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                                       .Include(a => a.Products)
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
