using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StoreWebApi.Services;
using StoreWebApi.Helpers;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService= productService;
        }

        [HttpGet]
        [Route("GetProducts")]
        public IActionResult GetProducts(int categoryId=0, bool isOnlyInStock = false,double minPrice = 0, double maxPrice = 0)
        { 
            ValidateParameters(categoryId,minPrice,maxPrice);
            if (ModelState.ErrorCount> 0)
            {
                return this.GetBadRequest("Не корректно переданы параметры запроса");
            }

            var products = _productService.GetProducts(categoryId,isOnlyInStock,minPrice,maxPrice);

            if (products != null && products.Count>0)
            {
                return Ok(products);
            }
            return NotFound("По заданным критериям не удалось найти ни одного товара");
        }

        private void ValidateParameters(int categoryId, double minPrice, double maxPrice ) 
        {
            if (categoryId < 0)
            {
                ModelState.AddModelError("InvalidCategoryId",$"Id категории указано не корректно!{Environment.NewLine}");
            }
            if (maxPrice < 0) 
            {
                ModelState.AddModelError("InvalidMaxPrice", $"Максимальная цена не может быть отрицательной{Environment.NewLine}");
            }
            if (minPrice < 0)
            {
                ModelState.AddModelError("InvalidMinPrice", $"Минимальная цена не может быть отрицательной{Environment.NewLine}");
            }
        }
    }
}
