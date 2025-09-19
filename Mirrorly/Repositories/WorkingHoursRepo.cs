using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class WorkingHoursRepo : IWorkingHoursRepo
    {
        private readonly ProjectExeContext _context;

        public WorkingHoursRepo(ProjectExeContext context)
        {
            _context = context;
        }

        public List<WorkingHour> getWorkingHoursByMuaId(int id)
        {
            return _context.WorkingHours.Where(w => w.MuaId == id).ToList();
        }
    }
}
