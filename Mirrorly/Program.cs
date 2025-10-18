using CloudinaryDotNet;
using dotenv.net;
using Mirrorly.Middleware;
using WebCozyShop.Infrastructure;

namespace Mirrorly
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

            var builder = WebApplication.CreateBuilder(args);

            Account account = new Account(
    Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
    Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
    Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET"));

            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;
            builder.Services.AddSingleton(cloudinary);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Add services to the container.
            builder.Services.AddDbContextLayer(builder.Configuration);
            builder.Services.AddThirdPartyIntegrations(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable session before auth middleware
            app.UseSession();
            
            // Add custom session auth middleware
            app.UseSessionAuth();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
