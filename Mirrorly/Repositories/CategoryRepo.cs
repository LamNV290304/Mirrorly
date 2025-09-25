using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ProjectExeContext _context;

        public CategoryRepo(ProjectExeContext context)
        {
            _context = context;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.Include(s => s.Services).ToList();
        }
    }
}
