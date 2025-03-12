using BookShopDomain.Model;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookShopInfrastructure.Services
{
    public class ProductExportService : IExportService<Product>
    {
        private readonly BookShopContext _context;

        public ProductExportService(BookShopContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");

                // Створення заголовків стовпців
                worksheet.Cell(1, 1).Value = "Назва";
                worksheet.Cell(1, 2).Value = "Опис";
                worksheet.Cell(1, 3).Value = "Рік";
                worksheet.Cell(1, 4).Value = "Ціна";
                worksheet.Cell(1, 5).Value = "Продавець";
                worksheet.Cell(1, 6).Value = "Статус";
                worksheet.Cell(1, 7).Value = "Автори";
                worksheet.Cell(1, 8).Value = "Жанри";

                // Отримуємо продукти з бази даних
                var products = await _context.Products
                    .Include(p => p.Seller)
                    .Include(p => p.BookStatus)
                    .Include(p => p.Authors)
                    .Include(p => p.Genres)
                    .ToListAsync(cancellationToken);

                // Записуємо дані про продукти в Excel
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cell(row, 1).Value = product.Name;
                    worksheet.Cell(row, 2).Value = product.Description;
                    worksheet.Cell(row, 3).Value = product.Year;
                    worksheet.Cell(row, 4).Value = product.Price;
                    worksheet.Cell(row, 5).Value = product.Seller.Name;
                    worksheet.Cell(row, 6).Value = product.BookStatus.Name;
                    worksheet.Cell(row, 7).Value = string.Join(", ", product.Authors.Select(a => a.Name));
                    worksheet.Cell(row, 8).Value = string.Join(", ", product.Genres.Select(g => g.Name));

                    row++;
                }

                // Збереження в потік
                await Task.Run(() => workbook.SaveAs(stream), cancellationToken);
            }
        }
    }
}
