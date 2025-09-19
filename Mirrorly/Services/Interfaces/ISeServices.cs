using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface ISeServices
    {
        public List<Service> getServicesByMuaId(int muaId);
    }
}
