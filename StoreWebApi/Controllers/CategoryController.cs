using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Services;

namespace StoreWebApi.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService categoryService)
        {
            _categoryService= categoryService;
        }

        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategories();

            return Ok(result);
        }

        public async Task<IActionResult> GetCategoryById(int id) 
        {
            var result = await _categoryService.GetCategoryById(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound($"В БД нет категории с ID = {id}");
        }
    }
}
