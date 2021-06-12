using System.Text;
using DevTracker.API.Features.Identity.Factories;
using DevTracker.API.Infrastructure.PipelineBehaviours;
using DevTracker.API.Infrastructure.Services;
using DevTracker.DAL;
using DevTracker.DAL.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DevTracker.API.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "DevTracker.API", Version = "v1"});
            });

            return services;
        }

        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            var appSettings = GetAppSettings(services, config);
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = GetAppSettings(services, config).ConnectionString;
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        public static AppSettings GetAppSettings(this IServiceCollection services, IConfiguration config)
        {
            var appSettingsConfig = config.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsConfig);
            return appSettingsConfig.Get<AppSettings>();
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = ValidationConstants.User.MinimumPasswordLength;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
            ;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>))
                .AddScoped<TokenFactory>()
                .AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
