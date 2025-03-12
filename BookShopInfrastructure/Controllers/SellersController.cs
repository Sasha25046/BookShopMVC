using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShopDomain.Model;
using BookShopInfrastructure;

namespace BookShopInfrastructure.Controllers
{
    public class SellersController : Controller
    {
        private readonly BookShopContext _context;

        public SellersController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Sellers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sellers.ToListAsync());
        }

        // GET: Sellers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // GET: Sellers/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Sellers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] Seller seller)
        {
            // Перевірка на дублювання за іменем продавця
            var existingSeller = await _context.Sellers
                                                .FirstOrDefaultAsync(s => s.Name == seller.Name);

            if (existingSeller != null)
            {
                ModelState.AddModelError(string.Empty, "Продавець з таким ім'ям вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(seller);
        }


        // GET: Sellers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }
            return View(seller);
        }


        // POST: Sellers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] Seller seller)
        {
            if (id != seller.Id)
            {
                return NotFound();
            }

            // Перевірка на дублювання (за винятком поточного продавця)
            var existingSeller = await _context.Sellers
                                                .Where(s => s.Name == seller.Name && s.Id != id)
                                                .FirstOrDefaultAsync();

            if (existingSeller != null)
            {
                ModelState.AddModelError(string.Empty, "Продавець з таким ім'ям вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seller);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerExists(seller.Id))
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
            return View(seller);
        }


        // GET: Sellers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                                       .Include(s => s.Products)  
                                       .FirstOrDefaultAsync(m => m.Id == id);
            if (seller == null)
            {
                return NotFound();
            }

            if (seller.Products.Any())  
            {
                ViewBag.ErrorMessage = "Неможливо видалити постачальника, оскільки він має асоційовані товари.";
                return View("DeleteError", seller);
            }

            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seller = await _context.Sellers
                                       .Include(s => s.Products) 
                                       .FirstOrDefaultAsync(m => m.Id == id);

            if (seller == null)
            {
                return NotFound();
            }

            if (seller.Products.Any())  
            {
                TempData["ErrorMessage"] = "Неможливо видалити постачальника, оскільки він має товари.";
                return RedirectToAction(nameof(Index));  
            }

            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Постачальник успішно видалений.";
            return RedirectToAction(nameof(Index)); 
        }

        private bool SellerExists(int id)
        {
            return _context.Sellers.Any(e => e.Id == id);
        }
    }
}
