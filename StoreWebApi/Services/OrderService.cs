using Microsoft.Extensions.Caching.Memory;
using StoreWebApi.Helpers;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class OrderService
    {
        private readonly ApplicationContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;
        private readonly ProductService _productService;

        public OrderService(ApplicationContext applicationContext, IMemoryCache cache, ProductService productService)
        {
            _dbContext= applicationContext;
            _cache= cache;
            _cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
            _productService= productService;
        }

        public List<Order>? GetOrdersByCustomerId(int customerId)
        {
            List<Order>? orders;
            string cacheKey = $"{CacheKeys.OrdersByCustomerId}{customerId}";

            if (!_cache.TryGetValue(cacheKey, out orders))
            {
                orders = _dbContext.Orders.ToList().
                    Where(x => x.CustomerId == customerId).
                    OrderBy(x=>x.CreatedDate).
                    ToList();
                _cache.Set(CacheKeys.OrdersByCustomerId, orders,_cacheEntryOptions);
            }

            return orders;
        }


        public List<Order>? GetOrdersByCustomerId(int customerId, DateTime? dateFrom, DateTime? dateTo)
        {
            var orders = GetOrdersByCustomerId(customerId);
            if (orders != null && orders.Any())
            {
                if (dateFrom != null)
                {
                    orders = orders.Where(x => x.CreatedDate > dateFrom).ToList();
                }
                if (dateTo != null)
                {
                    orders = orders.Where(x => x.CreatedDate < dateTo).ToList();
                }
            }
            return orders;
        }

        public Order? GetOrderById(int id) 
        {
            Order? order;
            string cacheKey = $"{CacheKeys.OrderById}{id}";
            if (!_cache.TryGetValue(cacheKey, out order))
            {
                order = _dbContext.Orders.FirstOrDefault(x => x.Id == id); 
                _cache.Set(cacheKey, order, _cacheEntryOptions);
            }
            return order;
        }
        public List<OrderProduct>? GetOrderProductsByOrderId(int id)
        {
            List<OrderProduct>? orderProducts;
            string cacheKey = $"{CacheKeys.OrderById}{id}";
            if (!_cache.TryGetValue(cacheKey, out orderProducts))
            {
                orderProducts = _dbContext.OrderProducts.ToList().Where(x => x.OrderId == id).ToList();
                _cache.Set(cacheKey, orderProducts, _cacheEntryOptions);
            }
            return orderProducts;
        }

        public void CreateOrder(List<Product>products, int customerId)
        {
            Order order = new Order()
            {
                CreatedDate = DateTime.Now,
                CustomerId = customerId
            };

            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();

            AddProductsToOrder(products, order.Id);
        }

        private void AddProductsToOrder(List<Product> products, int orderId)
        {
            int positionId = 0;
            var productIds = products.Select(x=> x.Id).ToList();
            var productsDb = _dbContext.Products.Where(x=> productIds.Contains(x.Id)).ToList(); 

            foreach(var product in products) 
            {
                positionId++;
                var productDb = productsDb.Where(x=>x.Id== product.Id).First();

                OrderProduct orderProduct = new OrderProduct()
                {
                    PositionId= positionId,
                    ProductId= product.Id,
                    OrderId= orderId,
                    ProductPrice = productDb.Price,
                    ProductCount = product.Count
                };

                _dbContext.OrderProducts.Add(orderProduct);
            }
            _dbContext.SaveChanges();
            ChangeProductsCount(products, productsDb);
        }

        private void ChangeProductsCount(List<Product> products, List<ProductDb> productsDb) 
        {
            foreach (ProductDb productDb in productsDb)
            {
                var product = products.Where(x=>x.Id == productDb.Id).First();
                productDb.Count -= product.Count;

                _dbContext.Products.Update(productDb);
                _productService.UpdateProductCache(productDb);
            }
            _dbContext.SaveChanges();
        }
    }
}
