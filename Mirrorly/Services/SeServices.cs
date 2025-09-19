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

        public List<Service> getServicesByMuaId(int muaId)
        {
            return _serviceRepo.getServicesByMuaId(muaId);
        }
    }
}
