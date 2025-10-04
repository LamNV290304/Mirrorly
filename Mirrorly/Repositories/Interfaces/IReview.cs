using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IReview
    {
        List<Review> getReviewsByServiceId(int id);
        void addReview(Review review);
    }
}
