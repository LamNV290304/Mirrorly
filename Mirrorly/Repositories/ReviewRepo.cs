using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class ReviewRepo : IReview
    {
        private readonly ProjectExeContext _context;

        public ReviewRepo(ProjectExeContext context)
        {
            _context = context;
        }

        public void addReview(Review review)
        {
            _context.Add(review);
            _context.SaveChanges();
        }

        // Toàn đã sửa của Vũ
        public List<Review> getReviewsByServiceId(int muaId) 
        {
            return _context.Reviews
            .Include(r => r.Customer) 
            .Where(r => r.MuaId == muaId) 
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
        }
    }
}
