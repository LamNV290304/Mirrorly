using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class CategoryServices : ICategoryServices
    {
        private readonly ICategoryRepo _repo;

        public CategoryServices(ICategoryRepo repo)
        {
            _repo = repo;
        }

        public List<Category> GetAllCategories()
        {
           return _repo.GetAllCategories();
        }
    }
}
