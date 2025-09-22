using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IReview
    {
        List<Review> getReviewsById(int muaId);
        void addReview(Review review);
    }
}
