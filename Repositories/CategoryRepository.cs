using CategorySys.DTO;
using CategorySys.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace CategorySys.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string _cacheKey = "Categories";

        public CategoryRepository(AppDbContext context, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            if (!_memoryCache.TryGetValue(_cacheKey, out List<Category> categories))
            {
                categories = _context.Categories
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.UpdatedByUser)
                    .Include(c => c.ChildCategories)
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

                _memoryCache.Set(_cacheKey, categories, cacheEntryOptions);
            }

            return categories;
        }

        public List<Category> GetCache()
        {
            if (_memoryCache.TryGetValue(_cacheKey, out List<Category> cachedCategories))
            {
                return cachedCategories;
            }

            return null;
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
                UpdateCache();
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
                UpdateCache();
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
                    UpdateCache();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateCache()
        {
            var categories = _context.Categories
                .Include(c => c.CreatedByUser)
                .Include(c => c.UpdatedByUser)
                .Include(c => c.ChildCategories)
                .ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(60));

            _memoryCache.Set(_cacheKey, categories, cacheEntryOptions);
        }
    }
}
