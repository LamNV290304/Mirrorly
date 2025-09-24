using Microsoft.EntityFrameworkCore;
using Mirrorly.Models;
using Mirrorly.Services.Interfaces;

namespace Mirrorly.Services
{
    public class ComprehensiveProfileServices : IComprehensiveProfileServices
    {
        private readonly ProjectExeContext _context;

        public ComprehensiveProfileServices(ProjectExeContext context)
        {
            _context = context;
        }

        // Service Management
        public async Task<List<Service>> GetMuaServicesAsync(int muaId)
        {
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => s.MuaId == muaId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceAsync(long serviceId, int muaId)
        {
            return await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId && s.MuaId == muaId);
        }

        public async Task<bool> CreateServiceAsync(Service service)
        {
            try
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateServiceAsync(Service service)
        {
            try
            {
                _context.Services.Update(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteServiceAsync(long serviceId, int muaId)
        {
            try
            {
                var service = await GetServiceAsync(serviceId, muaId);
                if (service == null) return false;

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleServiceStatusAsync(long serviceId, int muaId)
        {
            try
            {
                var service = await GetServiceAsync(serviceId, muaId);
                if (service == null) return false;

                service.Active = !service.Active;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Portfolio Management
        public async Task<List<PortfolioItem>> GetMuaPortfolioAsync(int muaId)
        {
            return await _context.PortfolioItems
                .Where(p => p.Muaid == muaId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<PortfolioItem?> GetPortfolioItemAsync(long itemId, int muaId)
        {
            return await _context.PortfolioItems
                .FirstOrDefaultAsync(p => p.ItemId == itemId && p.Muaid == muaId);
        }

        public async Task<bool> CreatePortfolioItemAsync(PortfolioItem item)
        {
            try
            {
                item.CreatedAtUtc = DateTime.UtcNow;
                _context.PortfolioItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePortfolioItemAsync(PortfolioItem item)
        {
            try
            {
                _context.PortfolioItems.Update(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePortfolioItemAsync(long itemId, int muaId)
        {
            try
            {
                var item = await GetPortfolioItemAsync(itemId, muaId);
                if (item == null) return false;

                _context.PortfolioItems.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Working Hours Management
        public async Task<List<WorkingHour>> GetMuaWorkingHoursAsync(int muaId)
        {
            return await _context.WorkingHours
                .Where(w => w.MuaId == muaId)
                .OrderBy(w => w.DayOfWeek)
                .ToListAsync();
        }

        public async Task<bool> UpdateWorkingHoursAsync(int muaId, List<WorkingHour> workingHours)
        {
            try
            {
                // Remove existing working hours
                var existingHours = await _context.WorkingHours
                    .Where(w => w.MuaId == muaId)
                    .ToListAsync();

                _context.WorkingHours.RemoveRange(existingHours);

                // Add new working hours
                foreach (var hour in workingHours)
                {
                    hour.MuaId = muaId;
                    _context.WorkingHours.Add(hour);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsAvailableAsync(int muaId, DateTime startTime, DateTime endTime)
        {
            var dayOfWeek = (byte)((int)startTime.DayOfWeek == 0 ? 7 : (int)startTime.DayOfWeek);

            var workingHour = await _context.WorkingHours
                .FirstOrDefaultAsync(w => w.MuaId == muaId && w.DayOfWeek == dayOfWeek);

            if (workingHour == null) return false;

            var startTimeOnly = TimeOnly.FromDateTime(startTime);
            var endTimeOnly = TimeOnly.FromDateTime(endTime);

            return startTimeOnly >= workingHour.StartTime && endTimeOnly <= workingHour.EndTime;
        }

        // Categories
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        // Profile Validation
        public async Task<bool> IsProfileCompleteAsync(int muaId)
        {
            var status = await GetProfileCompletionStatusAsync(muaId);
            return status.IsComplete;
        }

        public async Task<ProfileCompletionStatus> GetProfileCompletionStatusAsync(int muaId)
        {
            var status = new ProfileCompletionStatus();
            var missingItems = new List<string>();

            // Check basic info
            var profile = await _context.Muaprofiles.FindAsync(muaId);
            status.HasBasicInfo = profile != null &&
                                 !string.IsNullOrWhiteSpace(profile.DisplayName) &&
                                 !string.IsNullOrWhiteSpace(profile.Bio) &&
                                 !string.IsNullOrWhiteSpace(profile.AddressLine);

            if (!status.HasBasicInfo)
                missingItems.Add("Thông tin cơ bản (tên, giới thiệu, địa chỉ)");

            // Check services
            var servicesCount = await _context.Services.CountAsync(s => s.MuaId == muaId && s.Active);
            status.HasServices = servicesCount > 0;
            if (!status.HasServices)
                missingItems.Add("Ít nhất 1 dịch vụ");

            // Check portfolio
            var portfolioCount = await _context.PortfolioItems.CountAsync(p => p.Muaid == muaId);
            status.HasPortfolio = portfolioCount >= 3; // Require at least 3 portfolio items
            if (!status.HasPortfolio)
                missingItems.Add($"Portfolio (cần ít nhất 3 hình ảnh, hiện có {portfolioCount})");

            // Check working hours
            var workingHoursCount = await _context.WorkingHours.CountAsync(w => w.MuaId == muaId);
            status.HasWorkingHours = workingHoursCount > 0;
            if (!status.HasWorkingHours)
                missingItems.Add("Lịch làm việc");

            status.MissingItems = missingItems;

            // Calculate completion percentage
            var completedItems = 0;
            if (status.HasBasicInfo) completedItems++;
            if (status.HasServices) completedItems++;
            if (status.HasPortfolio) completedItems++;
            if (status.HasWorkingHours) completedItems++;

            status.CompletionPercentage = (completedItems * 100) / 4;

            return status;
        }
    }
}