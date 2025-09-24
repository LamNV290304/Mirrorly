using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IServiceRepo
    {
        public List<Service> getServicesByMuaId(int muaId);
        Task AddAsync(Service service);
        Task UpdateAsync(Service service);
        Task DeleteAsync(long id);
        Task<Service?> GetByIdAsync(int id);
    }
}
