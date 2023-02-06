using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Helpers;
using StoreWebApi.Models;
using StoreWebApi.Services;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;
        public CustomersController(CustomerService service)
        {
            _customerService = service;
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        public IActionResult GetAllCustomers()
        {
            var result = _customerService.GetAllCustomers();
            if (result == null || result.Count>0)
            {
                return Ok(result);
            }
            return NotFound(new { Message = "В базе данных нет ни одного клиента" });
        }

        [HttpGet]
        [Route("GetCustomerById")]
        public IActionResult GetCustomerById(int id)
        {
            var result = _customerService.GetCustomerById(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound(new { Message = $"В базе данных нет ни одного пользователя с Id = {id}" });
        }


        [HttpPost]
        [Route("CreateCustomer")]
        public IActionResult CreateCustomer([FromBody]Customer customer)
        {
            ValidateCustomerModel(customer);
            if (ModelState.ErrorCount > 0)
            {
                return this.GetBadRequest("Не корректно введены данные пользователя");
            }
            _customerService.CreateCustomer(customer);
            return Ok(new {message = "Создан новый клиент"});
        }

        //Для валидации имени так же можно использовать регулярные выражения (не хватило времени)
        private void ValidateCustomerModel(Customer customer)
        {
            if (customer.Name.Length > 50)
            {
                ModelState.AddModelError("IncorrectName",$"Слишком длинное имя клиента. Максимальная длинна 50 символов{Environment.NewLine}");
            }

            if (customer.TelephoneNumber.Length > 12 || !customer.TelephoneNumber.StartsWith("+7"))
            {
                ModelState.AddModelError("IncorrectTelephoneNumber", $"Номер телефона введён некорректно!{Environment.NewLine}");
            }
        }
    }
}
