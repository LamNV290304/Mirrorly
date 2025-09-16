
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mirrorly.Models;

namespace WebCozyShop.Infrastructure
{
    public static class DbContextServiceRegistration
    {
        public static IServiceCollection AddDbContextLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProjectExeContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MyCnn")));

            return services;
        }
    }
}
