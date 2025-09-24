using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class SeServices : ISeServices
    {
        private readonly IServiceRepo _serviceRepo;

        public SeServices(IServiceRepo serviceRepo)
        {
            _serviceRepo = serviceRepo;
        }

        public async Task addServiceAsync(Service service)
        {
          await _serviceRepo.AddAsync(service);
        }

        public async Task DeleteServiceAsync(long id)
        {
            await _serviceRepo.DeleteAsync(id);
        }
        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _serviceRepo.GetByIdAsync(id);
        }

        public List<Service> getServicesByMuaId(int muaId)
        {
            return _serviceRepo.getServicesByMuaId(muaId);
        }

        public async Task UpdateServiceAsync(Service service)
        {
            await _serviceRepo.UpdateAsync(service);
        }
    }
}
