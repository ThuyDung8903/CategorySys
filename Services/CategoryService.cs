using CategorySys.DTO;
using CategorySys.Models;
using CategorySys.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CategorySys.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _memoryCache;
        private const string _cacheKey = "Categories";

        public CategoryService(ICategoryRepository categoryRepository, IMemoryCache memoryCache)
        {
            _categoryRepository = categoryRepository;
            _memoryCache = memoryCache;
        }

        public void Add(CategoryDTO categoryDTO)
        {
            try
            {
                _categoryRepository.Add(categoryDTO);
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
                _categoryRepository.Delete(id);
                UpdateCache();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            try
            {
                if (!_memoryCache.TryGetValue(_cacheKey, out List<Category> cachedCategories))
                {
                    var categories = _categoryRepository.GetAllCategories();

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                    _memoryCache.Set(_cacheKey, categories, cacheEntryOptions);
                }
                if (cachedCategories == null)
                {
                    return _categoryRepository.GetAllCategories();
                }

                return cachedCategories;
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
                _categoryRepository.Update(id, categoryDTO);
                UpdateCache();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateCache()
        {
            var categories = _categoryRepository.GetAllCategories();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _memoryCache.Set(_cacheKey, categories, cacheEntryOptions);

        }

        public List<Category> GetCache()
        {
            if (_memoryCache.TryGetValue(_cacheKey, out List<Category> cachedCategories))
            {
                return cachedCategories;
            }
            return null;
        }
    }
}
