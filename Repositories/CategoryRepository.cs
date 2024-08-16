using CategorySys.DTO;
using CategorySys.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CategorySys.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            var categories = _context.Categories
                 .Include(c => c.CreatedByUser)
                 .Include(c => c.UpdatedByUser)
                 .Include(c => c.ChildCategories)
                 .ToList();

            return categories;
        }

        public void Add(CategoryDTO categoryDTO)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    throw new Exception("User ID not found");
                }

                var userId = int.Parse(userIdClaim.Value);

                var category = new Category
                {
                    Code = categoryDTO.Code,
                    Title = categoryDTO.Title,
                    ParentCategoryId = categoryDTO.ParentCategoryId,
                    CreatedBy = userId,
                };

                _context.Categories.Add(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(int id, CategoryDTO categoryDTO)
        {
            try
            {
                var category = _context.Categories.Find(id);

                if (category == null)
                {
                    throw new Exception("Category not found");
                }

                category.Title = categoryDTO.Title;
                category.Code = categoryDTO.Code;
                category.ParentCategoryId = categoryDTO.ParentCategoryId;
                category.UpdatedAt = DateTime.Now;
                category.UpdatedBy = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                _context.Categories.Update(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int id)
        {
            try
            {
                var category = _context.Categories.Find(id);
                if (category != null)
                {
                    var childCategories = _context.Categories.Where(c => c.ParentCategoryId == id).ToList();

                    foreach (var childCategory in childCategories)
                    {
                        childCategory.ParentCategoryId = null;
                    }

                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
