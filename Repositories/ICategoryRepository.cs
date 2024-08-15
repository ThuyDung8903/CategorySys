using CategorySys.DTO;
using CategorySys.Models;

namespace CategorySys.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
        void Add(CategoryDTO categoryDTO);
        void Update(int id, CategoryDTO categoryDTO);
        void Delete(int id);
        List<Category> GetCache();
        void UpdateCache();

    }
}
