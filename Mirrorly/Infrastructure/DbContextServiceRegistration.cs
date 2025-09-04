namespace WebCozyShop.Infrastructure
{
    public static class DbContextServiceRegistration
    {
        public static IServiceCollection AddDbContextLayer(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<CozyShopContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
