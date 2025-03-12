using BookShopDomain.Model;
using BookShopInfrastructure.Services;
using System;

namespace BookShopInfrastructure.Services
{
    public class ProductDataPortServiceFactory
        : IDataPortServiceFactory<Product>
    {
        private readonly BookShopContext _context;

        public ProductDataPortServiceFactory(BookShopContext context)
        {
            _context = context;
        }

        public IImportService<Product> GetImportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new ProductImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for products with content type {contentType}");
        }

        public IExportService<Product> GetExportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new ProductExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for products with content type {contentType}");
        }
    }
}
