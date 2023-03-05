using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StoreWebApi.Helpers;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class CategoryService 
    {
        private readonly ApplicationContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public CategoryService(ApplicationContext dbContext, IMemoryCache cache) 
        {
            _dbContext= dbContext;
            _cache= cache;
            _cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
        }
        public async Task<IReadOnlyCollection<Category>> GetAllCategories()
        {
            IReadOnlyCollection<Category>? categories;

            if (!_cache.TryGetValue(CacheKeys.AllCategories, out categories))
            {
                categories = await _dbContext.Categories.ToListAsync();

                _cache.Set(CacheKeys.AllCustomers, categories, _cacheEntryOptions);
            }

            return categories ?? new List<Category>();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            Category? category;
            string cacheKey = $"{CacheKeys.CategoryById}{id}";

            if (!_cache.TryGetValue(cacheKey, out category))
            {
                category = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

                _cache.Set(CacheKeys.AllCustomers, category, _cacheEntryOptions);
            }

            return category;
        }
    }
}
