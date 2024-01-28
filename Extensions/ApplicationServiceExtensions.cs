using DwitterSocial.Data;
using DwitterSocial.Helpers;
using DwitterSocial.Interfaces;
using DwitterSocial.Services;
using DwitterSocial.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DwitterSocial.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) 
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddTransient<PresenceTracker>();
            services.AddTransient<AppSeeder>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<LogUserActive>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("Dev"));
            });

            return services;
        }
    }
}
