using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Application.Activities;
using Application.Core;
using AutoMapper;
using API.Extensions;
using FluentValidation.AspNetCore;
using API.Middleware;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using API.SignalR;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            //sets a connection string
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        //This will also recognize the validator, use it for the create Activity handler. 
        public void ConfigureServices(IServiceCollection services)
        {
            //opt means options
            //This wil lmake it so that all API calls require authentication, unless we tell it otherwise
            services.AddControllers(opt => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(ConfigurationBinder =>
            {
                ConfigurationBinder.RegisterValidatorsFromAssemblyContaining<Create>();
            });

            //This calls ApplicationServiceExtensions to get all servies
            services.AddApplicationServices(_config);
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //exception handling middleware
            app.UseMiddleware<ExceptionMiddleware>();

            // app.UseHttpsRedirection();

            app.UseRouting();

            //Allows CORS policy to prevent issues
            app.UseCors("CorsPolicy");

            //Authenticating MUST come before authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoint used to connect to ChatHub
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
