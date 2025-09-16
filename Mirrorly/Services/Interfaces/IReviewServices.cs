using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IReviewServices
    {
        List<Review> getReviewsById(int muaId);
    }
}
