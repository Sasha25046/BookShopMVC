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
    public class StatusController : Controller
    {
        private readonly BookShopContext _context;

        public StatusController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Status
        public async Task<IActionResult> Index()
        {
            var statuses = await _context.Statuses.ToListAsync();
            return View(statuses);
        }

        // GET: Status/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        // GET: Status/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: Status/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id")] Status status)
        {
            var existingStatus = await _context.Statuses
                                                .FirstOrDefaultAsync(s => s.Name == status.Name);

            if (existingStatus != null)
            {
                ModelState.AddModelError(string.Empty, "Статус з таким ім'ям вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(status);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(status);
        }

        // GET: Status/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses.FindAsync(id);
            if (status == null)
            {
                return NotFound();
            }
            return View(status);
        }


        // POST: Status/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id")] Status status)
        {
            if (id != status.Id)
            {
                return NotFound();
            }

            var existingStatus = await _context.Statuses
                                                .Where(s => s.Name == status.Name && s.Id != id)
                                                .FirstOrDefaultAsync();

            if (existingStatus != null)
            {
                ModelState.AddModelError(string.Empty, "Статус з таким ім'ям вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(status);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusExists(status.Id))
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
            return View(status);
        }


        // GET: Status/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        // POST: Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status != null)
            {
                _context.Statuses.Remove(status);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatusExists(int id)
        {
            return _context.Statuses.Any(e => e.Id == id);
        }
    }
}
