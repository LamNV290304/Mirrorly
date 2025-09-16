using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IServiceRepo
    {
        public List<Service> getServicesByMuaId(int muaId);
    }
}
