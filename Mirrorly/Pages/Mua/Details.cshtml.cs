using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirrorly.Models;
using Mirrorly.Services;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Pages.Mua
{
    public class DetailsModel : PageModel
    {
        private readonly IReviewServices _review;
        private readonly ISeServices _service;
        private readonly IPortfoServices _portfo;
        private readonly IWorkingHoursServices _workingHours;

        public DetailsModel(IReviewServices review, ISeServices service, IPortfoServices portfo, IWorkingHoursServices workingHours)
        {
            _review = review;
            _service = service;
            _portfo = portfo;
            _workingHours = workingHours;
        }
        [BindProperty]
        public List<Review> Reviews { get; set; }
        public List<Service> Services { get; set; }
        public List<PortfolioItem> Portfolios { get; set; }
        public List<WorkingHour> WorkingHours { get; set; }
        public void OnGet(int id)
        {
            Reviews = _review.getReviewsById(id);
            Services = _service.getServicesByMuaId(id);
            Portfolios = _portfo.getPortfolioItemsByMuaId(id);
            WorkingHours = _workingHours.GetWorkingHoursByMuaId(id);
        }
    }
}
