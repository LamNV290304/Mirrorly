using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IPortfoServices
    {
        public List<PortfolioItem> getPortfolioItemsByMuaId(int id);
    }
}
