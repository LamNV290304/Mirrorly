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

        public List<Review> getReviewsByServiceId(int id)
        {
            return _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.Booking)
            .Where(r => r.Booking.ServiceId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
        }
    }
}
