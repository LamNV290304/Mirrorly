using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IReview
    {
        List<Review> getReviewsByMuaId(int id);
        void addReview(Review review);
    }
}
