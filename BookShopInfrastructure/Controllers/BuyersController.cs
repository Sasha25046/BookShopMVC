using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookShopInfrastructure;
using BookShopDomain.Model;

namespace BookShopInfrastructure.Controllers
{
    public class BuyersController : Controller
    {
        private readonly BookShopContext _context;

        public BuyersController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Buyers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Buyers.ToListAsync());
        }

        // GET: Buyers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var buyer = await _context.Buyers
                                      .Include(b => b.Orders)
                                      .ThenInclude(o => o.Status)
                                      .FirstOrDefaultAsync(m => m.Id == id);
            if (buyer == null) return NotFound();

            return View(buyer);
        }

        // GET: Buyers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buyers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Address")] Buyer buyer)
        {
            if (_context.Buyers.Any(b => b.Name == buyer.Name))
            {
                ModelState.AddModelError("Name", "Такий покупець вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(buyer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(buyer);
        }

        // GET: Buyers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var buyer = await _context.Buyers.FindAsync(id);
            if (buyer == null) return NotFound();

            return View(buyer);
        }

        // POST: Buyers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Buyer buyer)
        {
            if (id != buyer.Id) return NotFound();

            if (_context.Buyers.Any(b => b.Name == buyer.Name && b.Id != buyer.Id))
            {
                ModelState.AddModelError("Name", "Такий покупець вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(buyer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuyerExists(buyer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(buyer);
        }

        // GET: Buyers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var buyer = await _context.Buyers
                                      .Include(b => b.Orders)
                                      .FirstOrDefaultAsync(m => m.Id == id);
            if (buyer == null) return NotFound();

            if (buyer.Orders.Any())
            {
                ViewBag.ErrorMessage = "Неможливо видалити покупця, оскільки у нього є замовлення.";
                return View("DeleteError", buyer);
            }

            return View(buyer);
        }

        // POST: Buyers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buyer = await _context.Buyers
                                      .Include(b => b.Orders)
                                      .FirstOrDefaultAsync(m => m.Id == id);

            if (buyer == null) return NotFound();

            if (buyer.Orders.Any())
            {
                TempData["ErrorMessage"] = "Неможливо видалити покупця, оскільки у нього є замовлення.";
                return RedirectToAction(nameof(Index));
            }

            _context.Buyers.Remove(buyer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Покупець успішно видалений.";
            return RedirectToAction(nameof(Index));
        }

        private bool BuyerExists(int id)
        {
            return _context.Buyers.Any(e => e.Id == id);
        }
    }
}
