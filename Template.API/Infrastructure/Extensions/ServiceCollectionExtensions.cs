using System;
using System.Text;
using FluentValidation.AspNetCore;
using Template.API.Features.Identity.Factories;
using Template.API.Infrastructure.PipelineBehaviours;
using Template.API.Infrastructure.Services;
using Template.DAL;
using Template.DAL.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Template.API.Infrastructure.Exceptions;

namespace Template.API.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Template.API", Version = "v1" });
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
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IExceptionLogger, ExceptionLogger>()
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>))
                .AddScoped<TokenFactory>()
                .AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IServiceCollection AddControllersWithValidation(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddFluentValidation(
                    options => options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            return services;
        }
    }
}