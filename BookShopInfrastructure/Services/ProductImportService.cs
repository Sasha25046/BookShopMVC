using ClosedXML.Excel;
using BookShopDomain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BookShopInfrastructure.Services
{
    public class ProductImportService : IImportService<Product>
    {
        private readonly BookShopContext _context;

        public ProductImportService(BookShopContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                var worksheet = workBook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new Exception("Файл не містить аркушів.");
                }

                foreach (var row in worksheet.RowsUsed().Skip(1))
                {
                    await AddProductAsync(row, cancellationToken);
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }



        private async Task<Seller> GetOrCreateSellerAsync(string sellerName, CancellationToken cancellationToken)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.Name == sellerName, cancellationToken);

            if (seller == null)
            {
                seller = new Seller { Name = sellerName };
                _context.Sellers.Add(seller);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return seller;
        }

        private async Task<BookStatus> GetOrCreateBookStatusAsync(string bookStatusName, CancellationToken cancellationToken)
        {
            var bookStatus = await _context.BookStatuses
                .FirstOrDefaultAsync(b => b.Name == bookStatusName, cancellationToken);

            if (bookStatus == null)
            {
                bookStatus = new BookStatus { Name = bookStatusName };
                _context.BookStatuses.Add(bookStatus);
                await _context.SaveChangesAsync(cancellationToken); 
            }

            return bookStatus;
        }

        private async Task AddProductAsync(IXLRow row, CancellationToken cancellationToken)
        {
            var productName = row.Cell(1).Value.ToString();
            if (string.IsNullOrWhiteSpace(productName))
            {
                return; 
            }

            var productDescription = row.Cell(2).Value.ToString();

            var year = int.TryParse(row.Cell(3).Value.ToString(), out int yearResult) && yearResult >= 1400
                ? yearResult
                : 0; 

            if (year == 0) return;

            var price = decimal.TryParse(row.Cell(4).Value.ToString(), out decimal priceResult) && priceResult > 0
                ? priceResult
                : 0m; 

            if (price == 0m) return;

            var sellerName = row.Cell(5).Value.ToString();
            if (string.IsNullOrWhiteSpace(sellerName))
            {
                return; 
            }

            var bookStatusName = row.Cell(6).Value.ToString();
            if (string.IsNullOrWhiteSpace(bookStatusName))
            {
                return;
            }

            var seller = await GetOrCreateSellerAsync(sellerName, cancellationToken);

            var bookStatus = await GetOrCreateBookStatusAsync(bookStatusName, cancellationToken);

            var existingProduct = await _context.Products
                .Where(p => p.Name == productName && p.SellerId == seller.Id && p.Year == year)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingProduct != null)
            {
                return;
            }

            var product = new Product
            {
                Name = productName,
                Description = productDescription,
                Year = year,
                Price = price,
                SellerId = seller.Id,
                BookStatusId = bookStatus.Id
            };

            var validationContext = new ValidationContext(product);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(product, validationContext, validationResults, true);

            if (!isValid)
            {
                return;
            }

            await GetAuthorsAsync(row, product, cancellationToken);
            await GetGenresAsync(row, product, cancellationToken);

            _context.Products.Add(product);
        }





        private async Task GetAuthorsAsync(IXLRow row, Product product, CancellationToken cancellationToken)
        {
            var authorsColumn = row.Cell(7).Value.ToString();
            if (!string.IsNullOrWhiteSpace(authorsColumn))
            {
                var authorNames = authorsColumn.Split(',').Select(a => a.Trim()).ToList();
                foreach (var authorName in authorNames)
                {
                    var author = await _context.Authors
                        .FirstOrDefaultAsync(a => a.Name == authorName, cancellationToken);

                    if (author == null)
                    {
                        author = new Author { Name = authorName };
                        _context.Authors.Add(author); 
                        await _context.SaveChangesAsync(cancellationToken); 
                    }

                    product.Authors.Add(author); 
                }
            }
        }

        private async Task GetGenresAsync(IXLRow row, Product product, CancellationToken cancellationToken)
        {
            var genresColumn = row.Cell(8).Value.ToString();
            if (!string.IsNullOrWhiteSpace(genresColumn))
            {
                var genreNames = genresColumn.Split(',').Select(g => g.Trim()).ToList();
                foreach (var genreName in genreNames)
                {
                    var genre = await _context.Genres
                        .FirstOrDefaultAsync(g => g.Name == genreName, cancellationToken);

                    if (genre == null)
                    {
                        genre = new Genre { Name = genreName };
                        _context.Genres.Add(genre); 
                        await _context.SaveChangesAsync(cancellationToken); 
                    }

                    product.Genres.Add(genre);
                }
            }
        }

    }
}
