using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface ICategoryRepo
    {
        public List<Category> GetAllCategories();
    }
}
