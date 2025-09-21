using Mirrorly.Models;

namespace Mirrorly.Services.Interfaces
{
    public interface IComprehensiveProfileServices
    {
        // Service Management
        Task<List<Service>> GetMuaServicesAsync(int muaId);
        Task<Service?> GetServiceAsync(long serviceId, int muaId);
        Task<bool> CreateServiceAsync(Service service);
        Task<bool> UpdateServiceAsync(Service service);
        Task<bool> DeleteServiceAsync(long serviceId, int muaId);
        Task<bool> ToggleServiceStatusAsync(long serviceId, int muaId);

        // Portfolio Management
        Task<List<PortfolioItem>> GetMuaPortfolioAsync(int muaId);
        Task<PortfolioItem?> GetPortfolioItemAsync(long itemId, int muaId);
        Task<bool> CreatePortfolioItemAsync(PortfolioItem item);
        Task<bool> UpdatePortfolioItemAsync(PortfolioItem item);
        Task<bool> DeletePortfolioItemAsync(long itemId, int muaId);

        // Working Hours Management
        Task<List<WorkingHour>> GetMuaWorkingHoursAsync(int muaId);
        Task<bool> UpdateWorkingHoursAsync(int muaId, List<WorkingHour> workingHours);
        Task<bool> IsAvailableAsync(int muaId, DateTime startTime, DateTime endTime);

        // Categories
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryAsync(int categoryId);

        // Profile Validation
        Task<bool> IsProfileCompleteAsync(int muaId);
        Task<ProfileCompletionStatus> GetProfileCompletionStatusAsync(int muaId);
    }

    public class ProfileCompletionStatus
    {
        public bool HasBasicInfo { get; set; }
        public bool HasServices { get; set; }
        public bool HasPortfolio { get; set; }
        public bool HasWorkingHours { get; set; }
        public bool IsComplete => HasBasicInfo && HasServices && HasPortfolio && HasWorkingHours;
        public int CompletionPercentage { get; set; }
        public List<string> MissingItems { get; set; } = new();
    }
}