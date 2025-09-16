using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class WorkingHoursServices : IWorkingHoursServices
    {
        private readonly IWorkingHoursRepo _repo;

        public WorkingHoursServices(IWorkingHoursRepo repo)
        {
            _repo = repo;
        }

        public List<WorkingHour> GetWorkingHoursByMuaId(int muaId)
        {
            return _repo.getWorkingHoursByMuaId(muaId);
        }
    }
}
