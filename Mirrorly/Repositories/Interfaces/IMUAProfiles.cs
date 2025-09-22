using Mirrorly.Models;

namespace Mirrorly.Repositories.Interfaces
{
    public interface IMUAProfiles
    {
        List<Muaprofile> GetAll();
        Muaprofile? GetById(int id);
    }
}
