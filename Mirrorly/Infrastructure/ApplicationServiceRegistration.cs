using Microsoft.Win32;
using Mirrorly.Repositories;
using Mirrorly.Repositories.Interfaces;
using Mirrorly.Services;
using Mirrorly.Services.Interfaces;

namespace WebCozyShop.Infrastructure
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IMUAProfiles, MuaProfiles>();
            services.AddScoped<IReview, ReviewRepo>();
            services.AddScoped<IServiceRepo, ServiceRepo>();
            services.AddScoped<IPortfoRepo, PortfoRepo>();
            services.AddScoped<IWorkingHoursRepo, WorkingHoursRepo>();
            // Register services
            services.AddScoped<IMuaServices, MuaServices>();
            services.AddScoped<IReviewServices, ReviewServices>();
            services.AddScoped<ISeServices, SeServices>();
            services.AddScoped<IPortfoServices, PortfoServices>();
            services.AddScoped<IWorkingHoursServices, WorkingHoursServices>();

            // Register new Auth and Profile services
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IProfileServices, ProfileServices>();
            return services;
        }
    }
}
