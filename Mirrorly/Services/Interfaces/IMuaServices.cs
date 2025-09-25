using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IMuaServices
    {
        List<Muaprofile> GetAllMUAProfiles();
        Muaprofile? GetMuaProfileById(int id);
    }
}
