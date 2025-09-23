using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface ISeServices
    {
        public List<Service> getServicesByMuaId(int muaId);
        Task<Service?> GetServiceByIdAsync(int id);
        Task addServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(long id);
    }
}
