using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IProfileServices
    {
        Task<bool> CreateCustomerProfile(CustomerProfile profile);
        Task<bool> CreateMuaProfile(Muaprofile profile);
        Task<bool> UpdateCustomerProfile(CustomerProfile profile);
        Task<bool> UpdateMuaProfile(Muaprofile profile);
        Task<CustomerProfile?> GetCustomerProfile(int userId);
        Task<Muaprofile?> GetMuaProfile(int userId);
    }
}