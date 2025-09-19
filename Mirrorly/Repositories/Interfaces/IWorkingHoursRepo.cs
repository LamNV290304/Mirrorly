using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IWorkingHoursRepo
    {
        public List<WorkingHour> getWorkingHoursByMuaId(int id);
    }
}
