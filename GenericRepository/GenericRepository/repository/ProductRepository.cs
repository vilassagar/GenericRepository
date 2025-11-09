using GenericRepository.Base_Entity;
using GenericRepository.Generic;
using Microsoft.EntityFrameworkCore;

namespace GenericRepository.repository
{
    // Product-specific repository interface
    public interface IProductRepository : IQueryableRepository<Product, int>
    {
        Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<Product> UpdateStockAsync(int productId, int newQuantity, CancellationToken cancellationToken = default);
        Task<Product> AdjustStockAsync(int productId, int adjustment, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetProductsByStockRangeAsync(int minStock, int maxStock, CancellationToken cancellationToken = default);
        Task<PagedResult<Product>> GetProductsSortedByPriceAsync(bool ascending, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<decimal> GetAveragePriceAsync(CancellationToken cancellationToken = default);
        Task<ProductStockSummary> GetStockSummaryAsync(CancellationToken cancellationToken = default);
    }
    public class ProductRepository(DbContext context) : EfRepository<Product, int>(context), IProductRepository
    {
        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.StockQuantity <= threshold && p.StockQuantity > 0)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.StockQuantity == 0)
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var term = searchTerm.ToLower();
            return await _dbSet
                .Where(p => p.Name.ToLower().Contains(term) ||
                           p.Description.ToLower().Contains(term))
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Product> UpdateStockAsync(int productId, int newQuantity, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {productId} not found.");

            product.StockQuantity = newQuantity;
            return await UpdateAsync(product, cancellationToken);
        }

        public async Task<Product> AdjustStockAsync(int productId, int adjustment, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {productId} not found.");

            product.StockQuantity = Math.Max(0, product.StockQuantity + adjustment);
            return await UpdateAsync(product, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByStockRangeAsync(int minStock, int maxStock, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.StockQuantity >= minStock && p.StockQuantity <= maxStock)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Product>> GetProductsSortedByPriceAsync(bool ascending, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = ascending
                ? _dbSet.OrderBy(p => p.Price)
                : _dbSet.OrderByDescending(p => p.Price);

            var totalCount = await _dbSet.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<decimal> GetAveragePriceAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AverageAsync(p => p.Price, cancellationToken);
        }

        public async Task<ProductStockSummary> GetStockSummaryAsync(CancellationToken cancellationToken = default)
        {
            var products = await _dbSet.ToListAsync(cancellationToken);

            return new ProductStockSummary
            {
                TotalProducts = products.Count,
                InStockProducts = products.Count(p => p.StockQuantity > 0),
                OutOfStockProducts = products.Count(p => p.StockQuantity == 0),
                LowStockProducts = products.Count(p => p.StockQuantity > 0 && p.StockQuantity <= 10),
                TotalStockValue = products.Sum(p => p.Price * p.StockQuantity),
                AverageStockQuantity = products.Count > 0 ? products.Average(p => p.StockQuantity) : 0
            };
        }
    }

    public class ProductStockSummary
    {
        public required int TotalProducts { get; init; }
        public required int InStockProducts { get; init; }
        public required int OutOfStockProducts { get; init; }
        public required int LowStockProducts { get; init; }
        public required decimal TotalStockValue { get; init; }
        public required double AverageStockQuantity { get; init; }
    }
}
