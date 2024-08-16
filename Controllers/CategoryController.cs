using CategorySys.DTO;
using CategorySys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategorySys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var categories = _categoryService.GetAllCategories();

            return Ok(categories);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add(CategoryDTO categoryDTO)
        {
            _categoryService.Add(categoryDTO);

            return Ok("Category added successfully");
        }

        [HttpGet("cache")]
        public IActionResult GetCache()
        {
            var cachedCategories = _categoryService.GetCache();
            if (cachedCategories != null)
                return Ok(cachedCategories);

            return NotFound("Cache not found");
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryDTO categoryDTO)
        {
            _categoryService.Update(id, categoryDTO);

            return Ok("Category updated successfully");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _categoryService.Delete(id);

            return Ok("Category deleted successfully");
        }

    }
}
