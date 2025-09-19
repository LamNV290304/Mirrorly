using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IWorkingHoursServices
    {
        public List<WorkingHour> GetWorkingHoursByMuaId(int muaId);
    }
}
