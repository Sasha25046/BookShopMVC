using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShopDomain.Model;
using BookShopInfrastructure;
using System.Linq;
using System.Threading.Tasks;
using BookShopInfrastructure.Services;

namespace BookShopInfrastructure.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BookShopContext _context;
        private readonly ProductDataPortServiceFactory _productDataPostServiceFactory;

        public ProductsController(BookShopContext context, ProductDataPortServiceFactory productDataPostServiceFactory)
        {
            _context = context;
            _productDataPostServiceFactory = productDataPostServiceFactory;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // GET: Products/Index
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                                         .Include(p => p.Seller)
                                         .Include(p => p.BookStatus)
                                         .Include(p => p.Genres)
                                         .Include(p => p.Authors)
                                         .ToListAsync();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["SellerId"] = new SelectList(_context.Sellers, "Id", "Name");
            ViewData["BookStatusId"] = new SelectList(_context.BookStatuses, "Id", "Name");
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name");
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Year,Price,SellerId,BookStatusId")] Product product, int[] selectedGenres, int[] selectedAuthors)
        {
            var seller = await _context.Sellers.FindAsync(product.SellerId);
            var bookStatus = await _context.BookStatuses.FindAsync(product.BookStatusId);

            product.Seller = seller;
            product.BookStatus = bookStatus;

            ModelState.Clear();
            TryValidateModel(product); 

            var existingProduct = await _context.Products
                                                .Where(p => p.Name == product.Name && p.SellerId == product.SellerId && p.Year == product.Year)
                                                .FirstOrDefaultAsync();

            if (existingProduct != null)
            {
                ModelState.AddModelError(string.Empty, "Продукт з такою назвою, продавцем та роком видання вже існує.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(product); 

                if (selectedGenres != null)
                {
                    foreach (var genreId in selectedGenres)
                    {
                        var genre = await _context.Genres.FindAsync(genreId);
                        if (genre != null)
                        {
                            product.Genres.Add(genre);
                        }
                    }
                }

                if (selectedAuthors != null)
                {
                    foreach (var authorId in selectedAuthors)
                    {
                        var author = await _context.Authors.FindAsync(authorId);
                        if (author != null)
                        {
                            product.Authors.Add(author);
                        }
                    }
                }

                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index)); 
            }

            ViewData["SellerId"] = new SelectList(_context.Sellers, "Id", "Name", product.SellerId);
            ViewData["BookStatusId"] = new SelectList(_context.BookStatuses, "Id", "Name", product.BookStatusId);
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name", selectedGenres);
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", selectedAuthors);

            return View(product);
        }




        // GET: Products/Edit/2
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                        .Include(p => p.Seller)
                                        .Include(p => p.BookStatus)
                                        .Include(p => p.Genres)
                                        .Include(p => p.Authors)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["SellerId"] = new SelectList(_context.Sellers, "Id", "Name", product.SellerId);
            ViewData["BookStatusId"] = new SelectList(_context.BookStatuses, "Id", "Name", product.BookStatusId);
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name", product.Genres.Select(g => g.Id));
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", product.Authors.Select(a => a.Id));

            return View(product);
        }

        // POST: Products/Edit/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Description, Year, Price, SellerId, BookStatusId")] Product product, int[] selectedGenres, int[] selectedAuthors)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            var seller = await _context.Sellers.FindAsync(product.SellerId);
            var bookStatus = await _context.BookStatuses.FindAsync(product.BookStatusId);

            product.Seller = seller;
            product.BookStatus = bookStatus;

            ModelState.Clear();
            TryValidateModel(product); 

            var existingProduct = await _context.Products
                                                .Where(p => p.Name == product.Name && p.SellerId == product.SellerId && p.Year == product.Year && p.Id != id)
                                                .FirstOrDefaultAsync();

            if (existingProduct != null)
            {
                ModelState.AddModelError(string.Empty, "Продукт з такою назвою, продавцем та роком видання вже існує.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product); 

                    product.Genres.Clear();
                    product.Authors.Clear();

                    if (selectedGenres != null)
                    {
                        foreach (var genreId in selectedGenres)
                        {
                            var genre = await _context.Genres.FindAsync(genreId);
                            if (genre != null)
                            {
                                product.Genres.Add(genre);
                            }
                        }
                    }

                    if (selectedAuthors != null)
                    {
                        foreach (var authorId in selectedAuthors)
                        {
                            var author = await _context.Authors.FindAsync(authorId);
                            if (author != null)
                            {
                                product.Authors.Add(author);
                            }
                        }
                    }

                    await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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

            ViewData["SellerId"] = new SelectList(_context.Sellers, "Id", "Name", product.SellerId);
            ViewData["BookStatusId"] = new SelectList(_context.BookStatuses, "Id", "Name", product.BookStatusId);
            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name", selectedGenres);
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name", selectedAuthors);

            return View(product);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                        .Include(p => p.Seller)
                                        .Include(p => p.BookStatus)
                                        .Include(p => p.Genres)
                                        .Include(p => p.Authors)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["Genres"] = new SelectList(_context.Genres, "Id", "Name");
            ViewData["Authors"] = new SelectList(_context.Authors, "Id", "Name");

            return View(product);
        }

        // POST: Products/AddGenreToProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Products/AddGenreToProduct")]
        public async Task<IActionResult> AddGenreToProduct(int id, int[] selectedGenres)
        {
            var product = await _context.Products
                                        .Include(p => p.Genres)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            if (selectedGenres != null)
            {
                foreach (var genreId in selectedGenres)
                {
                    var genre = await _context.Genres.FindAsync(genreId);
                    if (genre != null && !product.Genres.Contains(genre))
                    {
                        product.Genres.Add(genre);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = product.Id });
        }

        // POST: Products/AddAuthorToProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Products/AddAuthorToProduct")]
        public async Task<IActionResult> AddAuthorToProduct(int id, int[] selectedAuthors)
        {
            var product = await _context.Products
                                        .Include(p => p.Authors)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            if (selectedAuthors != null)
            {
                foreach (var authorId in selectedAuthors)
                {
                    var author = await _context.Authors.FindAsync(authorId);
                    if (author != null && !product.Authors.Contains(author))
                    {
                        product.Authors.Add(author);
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = product.Id });
        }



        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                        .Include(p => p.Seller)
                                        .Include(p => p.BookStatus)
                                        .Include(p => p.Genres)
                                        .Include(p => p.Authors)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                                         .Include(p => p.OrderItems)  
                                         .Include(p => p.Genres)      
                                         .Include(p => p.Authors)     
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.OrderItems.Any())
            {
                TempData["ErrorMessage"] = "Неможливо видалити продукт, оскільки він вже є в одному або декількох замовленнях.";
                return RedirectToAction(nameof(Index));  
            }

            product.Genres.Clear();
            product.Authors.Clear();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Продукт було успішно видалено.";
            return RedirectToAction(nameof(Index));  
        }


        // POST: Products/RemoveGenreFromProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Products/RemoveGenreFromProduct")]
        public async Task<IActionResult> RemoveGenreFromProduct(int productId, int genreId)
        {
            var product = await _context.Products
                                        .Include(p => p.Genres)
                                        .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var genre = product.Genres.FirstOrDefault(g => g.Id == genreId);
            if (genre != null)
            {
                product.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = productId });
        }

        // POST: Products/RemoveAuthorFromProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Products/RemoveAuthorFromProduct")]
        public async Task<IActionResult> RemoveAuthorFromProduct(int productId, int authorId)
        {
            var product = await _context.Products
                                        .Include(p => p.Authors)
                                        .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var author = product.Authors.FirstOrDefault(a => a.Id == authorId);
            if (author != null)
            {
                product.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = productId });
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken = default)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                TempData["ErrorMessage"] = "Будь ласка, виберіть файл для завантаження.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var importService = _productDataPostServiceFactory.GetImportService(fileExcel.ContentType);

                using var stream = fileExcel.OpenReadStream();
                await importService.ImportFromStreamAsync(stream, cancellationToken);

                TempData["SuccessMessage"] = "Дані успішно завантажено!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Сталася помилка при завантаженні: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",CancellationToken cancellationToken = default)
        {
            var exportService = _productDataPostServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;


            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"categiries_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }


    }
}
