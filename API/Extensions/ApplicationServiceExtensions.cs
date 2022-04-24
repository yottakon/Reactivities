using Application.Activities;
using Application.Core;
using Application.Interfaces;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        //Extension methods will return a collection of services
        //It will get all the services from Startup to here
        //This is to clean up Startup class
        //Static means you won't need to make a new instance of a class when using an extension method

        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
                                                            IConfiguration config)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });

            //Adds the database connection service from Persistence
            services.AddDbContext<DataContext>(opt =>
            {
                //used to get connection string and create an SQL conection
                //DefaultConnection is defined in appsettings.Development.json
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            }
            );
            services.AddCors(opt =>
            {
                //Gives CORS policy for any mehtod with any header form localhost. This will get rid of the CORS policy error
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000");
                });
            });
            //Tells MediatR where the handlers are
            services.AddMediatR(typeof(List.Handler).Assembly);
            //Tells where the mapping profiles are located
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            //Gets the currently logged in user's username anywhere in the application
            //This also keeps the login athentication token for all requests
            services.AddScoped<IUserAccessor, UserAccessor>();
            //New service for photo accessor
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            //Add Cloudinary settings from infrastructure
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            //Add SignalR service
            services.AddSignalR();

            return services;
        }
    }
}