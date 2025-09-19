using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class ReviewServices : IReviewServices
    {
        private readonly IReview _reviewRepo;

        public ReviewServices(IReview reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public List<Review> getReviewsById(int muaId)
        {
            return _reviewRepo.getReviewsById(muaId);
        }
    }
}
