using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShopDomain.Model;
using BookShopInfrastructure;

namespace BookShopInfrastructure.Controllers
{
    public class OrdersController : Controller
    {
        private readonly BookShopContext _context;

        public OrdersController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Orders
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Status)
                .ToList();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return RedirectToAction("Index", "OrderItems", new { orderId = order.Id });
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["BuyerId"] = new SelectList(_context.Buyers, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusId,BuyerId,OrderDate,DeliveryDate,Id")] Order order)
        {
            var buyer = await _context.Buyers.FindAsync(order.BuyerId);
            var status = await _context.Statuses.FindAsync(order.StatusId);

            order.Buyer = buyer;
            order.Status = status;

            ModelState.Clear();
            TryValidateModel(order);

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BuyerId"] = new SelectList(_context.Buyers, "Id", "Name", order.BuyerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", order.StatusId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewData["BuyerId"] = new SelectList(_context.Buyers, "Id", "Name", order.BuyerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", order.StatusId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatusId,BuyerId,OrderDate,DeliveryDate,Id")] Order order)
        {
            if (id != order.Id) return NotFound();

            var buyer = await _context.Buyers.FindAsync(order.BuyerId);
            var status = await _context.Statuses.FindAsync(order.StatusId);
            order.Buyer = buyer;
            order.Status = status;

            ModelState.Clear();
            TryValidateModel(order);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(_context.Buyers, "Id", "Name", order.BuyerId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", order.StatusId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Замовлення успішно видалено.";
            }
            return RedirectToAction(nameof(Index));
        }


        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
