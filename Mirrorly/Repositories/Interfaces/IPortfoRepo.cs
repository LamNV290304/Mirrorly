using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IPortfoRepo
    {
        public List<PortfolioItem> getPortfoliosById(int muaId); 
    }
}
