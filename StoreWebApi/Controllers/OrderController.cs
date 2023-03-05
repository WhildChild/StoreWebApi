using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Helpers;
using StoreWebApi.Models;
using StoreWebApi.Services;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;

        public OrderController(OrderService orderService, ProductService productService, CustomerService customerService)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
        }

        [HttpGet]
        [Route("GetOrdersByCustomerId")]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerId, DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _orderService.GetOrdersByCustomerId(customerId,dateFrom,dateTo);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            var orderProducts = await _orderService.GetOrderProductsByOrderId(id);
            if (order != null && orderProducts !=null)
            {
                return Ok(new 
                {
                    Order = order,
                    OrderProducts = orderProducts
                });
            }
            return NotFound(new {Message= $"Заказа с Id {id} нет в базе данных"});
        }

        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] List<Product>productsList, int customerId)
        {
            ValidateProductList(productsList);
            ValidateCustomer(customerId);

            if (ModelState.ErrorCount > 0)
            {
                return this.GetBadRequest("Не корректно введены параметры");
            }

            await _orderService.CreateOrder(productsList,customerId);
            return Ok();
        }

        private void ValidateProductList(List<Product> productsList)
        {
            if (productsList == null) 
            {
                ModelState.AddModelError("InvalidProductList", $"Некорректно передан список продуктов!{Environment.NewLine}");
                return;
            }

            foreach (var product in productsList)
            {
                var productDb = _productService.GetProductById(product.Id);
                if (productDb == null )
                {
                    ModelState.AddModelError("InvalidProduct", $"Продукта с id {product.Id} нет в базе данных. Проверьте отправленный список продуктов{Environment.NewLine}");
                    break;
                }
                if (productDb.Count == 0 || productDb.Count<product.Count)
                {
                    ModelState.AddModelError("InvalidProduct", $"Продукта с id {product.Id} нет в наличии в нужном количестве");
                    break;
                }
            }
        }

        private void ValidateCustomer(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
            {
                ModelState.AddModelError("InvalidCustomerId", $"В базе данных нет ни одного пользователя с Id = {customerId} {Environment.NewLine}");
            }
        }
    }
}
