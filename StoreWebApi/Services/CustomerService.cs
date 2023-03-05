using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StoreWebApi.Helpers;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class CustomerService
    {
        private readonly ApplicationContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions cacheEntryOptions;

        public CustomerService(ApplicationContext context, IMemoryCache memoryCache) 
        {
            _dbContext = context;
            _cache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
        }

        public async Task<IReadOnlyCollection<CustomerDb>?> GetAllCustomers()
        {
           IReadOnlyCollection<CustomerDb>? customers;

            if (!_cache.TryGetValue(CacheKeys.AllCustomers, out customers))
            {
                customers = await GetAllCustomersFromDB();

                _cache.Set(CacheKeys.AllCustomers, customers, cacheEntryOptions);
            }

            return customers ?? new List<CustomerDb>();
        }

        public async Task<CustomerDb?> GetCustomerById(int id) 
        {
            CustomerDb? customer;
            string cacheKey = $"{CacheKeys.CustomerById}{id}";

            if (!_cache.TryGetValue(cacheKey, out customer))
            {
                customer = await _dbContext.Customers.SingleOrDefaultAsync(x => x.Id == id); 

                _cache.Set(CacheKeys.AllCustomers, customer, cacheEntryOptions);
            }

            return customer;
        }

        public async Task CreateCustomer(Customer customer)
        {
            CustomerDb customerDb = new CustomerDb()
            {
                Name = customer.Name,
                TelephoneNumber = customer.TelephoneNumber
            };

            _dbContext.Customers.Add(customerDb);
            _dbContext.SaveChanges();

           await UpdateCustomersCache();
        }

        private async Task UpdateCustomersCache()
        {
            IReadOnlyCollection<CustomerDb>? customers;
            string cacheKey = $"{CacheKeys.AllCustomers}";

            if (_cache.TryGetValue(cacheKey,out customers))
            {
                customers = await GetAllCustomersFromDB();
                _cache.Set(cacheKey, customers, cacheEntryOptions);
            }
        }
        private async Task<IReadOnlyCollection<CustomerDb>> GetAllCustomersFromDB()
        {
            return await _dbContext.Customers.ToListAsync(); 
        }
    }
}
