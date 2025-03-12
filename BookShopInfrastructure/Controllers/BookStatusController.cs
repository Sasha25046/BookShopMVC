using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BookShopInfrastructure;
using BookShopDomain.Model;

namespace BookShopInfrastructure.Controllers
{
    public class BookStatusController : Controller
    {
        private readonly BookShopContext _context;

        public BookStatusController(BookShopContext context)
        {
            _context = context;
        }

        // GET: BookStatus
        public async Task<IActionResult> Index()
        {
            var bookStatuses = await _context.BookStatuses.ToListAsync();
            return View(bookStatuses);
        }

        // GET: BookStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookStatus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] BookStatus bookStatus)
        {
            if (_context.BookStatuses.Any(bs => bs.Name == bookStatus.Name))
            {
                ModelState.AddModelError("Name", "Такий статус вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(bookStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookStatus);
        }

        // GET: BookStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bookStatus = await _context.BookStatuses.FindAsync(id);
            if (bookStatus == null) return NotFound();

            return View(bookStatus);
        }

        // POST: BookStatus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] BookStatus bookStatus)
        {
            if (id != bookStatus.Id) return NotFound();

            if (_context.BookStatuses.Any(bs => bs.Name == bookStatus.Name && bs.Id != bookStatus.Id))
            {
                ModelState.AddModelError("Name", "Такий статус вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookStatusExists(bookStatus.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookStatus);
        }

        // GET: BookStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bookStatus = await _context.BookStatuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookStatus == null) return NotFound();

            return View(bookStatus);
        }

        // POST: BookStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookStatus = await _context.BookStatuses.FindAsync(id);
            _context.BookStatuses.Remove(bookStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookStatusExists(int id)
        {
            return _context.BookStatuses.Any(e => e.Id == id);
        }
    }
}
