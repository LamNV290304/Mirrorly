using Mirrorly.Models;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class MuaServices : IMuaServices
    {
        private readonly IMUAProfiles _muaProfiles;

        public MuaServices(IMUAProfiles muaProfiles)
        {
            _muaProfiles = muaProfiles;
        }

        public List<Muaprofile> GetAllMUAProfiles()
        {
            return _muaProfiles.GetAll();
        }
    }
}
