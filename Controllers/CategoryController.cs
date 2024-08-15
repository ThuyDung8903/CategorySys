using CategorySys.DTO;
using CategorySys.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategorySys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_categoryRepository.GetAllCategories());
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post(CategoryDTO categoryDTO)
        {
            _categoryRepository.Add(categoryDTO);

            return Ok("Category added successfully");
        }

        [HttpGet("cache")]
        public IActionResult GetCache()
        {
            var cachedCategories = _categoryRepository.GetCache();
            if (cachedCategories != null)
                return Ok(cachedCategories);

            return NotFound("Cache not found");
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, CategoryDTO categoryDTO)
        {
            _categoryRepository.Update(id, categoryDTO);

            return Ok("Category updated successfully");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _categoryRepository.Delete(id);

            return Ok("Category deleted successfully");
        }

    }
}
