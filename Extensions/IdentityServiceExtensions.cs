using DwitterSocial.Data;
using DwitterSocial.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DwitterSocial.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = true;
            })  
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<AppDbContext>();
               
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })     
                   .AddJwtBearer(opt =>
                   {
                       opt.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidateIssuerSigningKey= true,
                          ValidateIssuer= false,
                          ValidateAudience = false,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Key").Value))
                       };

                       opt.Events = new JwtBearerEvents
                       {
                           OnMessageReceived = context =>
                           {
                               var accessToken = context.Request.Query["access_token"];

                               var path = context.HttpContext.Request.Path;
                               if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                               {
                                   context.Token = accessToken;
                               }

                               return Task.CompletedTask;
                           }
                       };
                   });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

            return services;
        }
    }
}
