using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShopDomain.Model;
using BookShopInfrastructure;

namespace BookShopInfrastructure.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly BookShopContext _context;

        public OrderItemsController(BookShopContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index(int orderId)
        {
            var orderItems = _context.OrderItems
                .Include(o => o.Product)
                .Where(o => o.OrderId == orderId);

            ViewBag.OrderId = orderId;
            return View(await orderItems.ToListAsync());
        }

        // GET: OrderItems/Create
        public IActionResult Create(int orderId)
        {
            ViewData["OrderId"] = orderId;
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View(new OrderItem { OrderId = orderId }); 
        }

        // POST: OrderItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Quantity,OrderId,Id")] OrderItem orderItem)
        {
            ModelState.Remove("Order");
            ModelState.Remove("Product");

            // Check if the OrderItem with the same ProductId already exists in the same Order
            bool exists = await _context.OrderItems
                .AnyAsync(o => o.OrderId == orderItem.OrderId && o.ProductId == orderItem.ProductId);

            if (exists)
            {
                ModelState.AddModelError("ProductId", "Товар вже доданий до цього замовлення.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { orderId = orderItem.OrderId });
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }


        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound();

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Quantity,OrderId,Id")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
                return NotFound();

            ModelState.Remove("Order");
            ModelState.Remove("Product");

            bool exists = await _context.OrderItems
                .AnyAsync(o => o.OrderId == orderItem.OrderId && o.ProductId == orderItem.ProductId && o.Id != orderItem.Id);

            if (exists)
            {
                ModelState.AddModelError("ProductId", "Товар вже доданий до цього замовлення.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { orderId = orderItem.OrderId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.OrderItems.Any(e => e.Id == orderItem.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", orderItem.ProductId);
            return View(orderItem);
        }



        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                                           .Include(o => o.Product)
                                           .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);  
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Товар успішно видалений з замовлення.";
            return RedirectToAction("Details", "Orders", new { id = orderItem.OrderId });
        }


        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }
    }
}
