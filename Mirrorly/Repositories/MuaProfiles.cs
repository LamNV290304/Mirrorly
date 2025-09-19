using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;

namespace Mirrorly.Repositories
{
    public class MuaProfiles : IMUAProfiles
    {
        private readonly ProjectExeContext _context;

        public MuaProfiles(ProjectExeContext context)
        {
            _context = context;
        }

        public List<Muaprofile> GetAll()
        {
            return _context.Muaprofiles.ToList();
        }
    }
}
