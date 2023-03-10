using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StoreWebApi.Helpers;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class ProductService
    {
        private readonly ApplicationContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public ProductService(ApplicationContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _cache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
        }

        public ProductDb? GetProductById(int id)
        {
            ProductDb? productDb;
            string cacheKey = $"{CacheKeys.ProductById}{id}";

            if (!_cache.TryGetValue(cacheKey, out productDb))
            {
                productDb = _dbContext.Products.FirstOrDefault(x => x.Id == id);
                if (productDb != null)
                    _cache.Set(cacheKey, productDb, _cacheOptions);
            }
            return productDb;
        }

        public async Task<IReadOnlyCollection<ProductDb>> GetProducts()
        {
            IReadOnlyCollection<ProductDb>? products;
            if (!_cache.TryGetValue($"{CacheKeys.AllProducts}", out products))
            {
                products = await GetProductsFromDb();
                _cache.Set($"{CacheKeys.AllProducts}", products, _cacheOptions);
            }

            return products ?? new List<ProductDb>();
        }

        public async Task<IReadOnlyCollection<ProductDb>> GetProducts(int categoryId = 0, bool isOnlyInStock = false, double minPrice = 0, double maxPrice = 0)
        {
            var products = await GetProducts();
            if (products != null)
            {
                if (categoryId != 0)
                {
                    products = products.Where(x => x.CategoryId == categoryId).ToList();
                }
                if (isOnlyInStock)
                {
                    products = products.Where(x => x.Count > 0).ToList();
                }
                if (maxPrice != 0)
                {
                    products = products.Where(x => x.Price < maxPrice).ToList();
                }
                products = products.Where(x => x.Price > minPrice).ToList();
            }

            return products ?? new List<ProductDb>();
        }

        public async Task UpdateProductCache(ProductDb productDb)
        {
            string cacheKey = $"{CacheKeys.ProductById}{productDb.Id}";
            ProductDb? product;

            if (_cache.TryGetValue(cacheKey, out product))
            {
                _cache.Set(cacheKey, productDb, _cacheOptions);
                IReadOnlyCollection<ProductDb> products = await GetProductsFromDb();
                _cache.Set($"{CacheKeys.AllProducts}", products, _cacheOptions);
            }
        }

        private async Task<IReadOnlyCollection<ProductDb>> GetProductsFromDb()
        {
            return await _dbContext.Products.ToListAsync();
        }
    }
}
