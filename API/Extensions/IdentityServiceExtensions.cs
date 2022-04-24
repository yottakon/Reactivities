using System.Text;
using System.Threading.Tasks;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration config)
            {
                services.AddIdentityCore<AppUser>(opt =>
                {
                    //passwird requirements. Dodes not need complex stuff
                    opt.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<DataContext>()
                .AddSignInManager<SignInManager<AppUser>>();
                //AddEntityFrameworkStores will register user and role stores with the application

                //This is the main key used for authentication. They user submitting authentication must match this key
                //This is the same key as the secret key for user login
                //Config is looking into appsettings.Development.json
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

                //this gives access to sign manager. The authentication service
                //JwtBearer is the type of authentication used
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt => {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            //This turn authentication on
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                        opt.Events = new JwtBearerEvents{
                            OnMessageReceived = context =>
                            {
                                //Clientside SignalR will get the token with the Query string 
                                var accessToken = context.Request.Query["access_token"];
                                var path = context.HttpContext.Request.Path;
                                //the endpoint /chat was declared in Startup.cs 
                                //If path matches the one for SignalR 
                                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                                {
                                    //get authentication token
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

                //Creates authorization policy IsActivtyHost
                //Add new IsHostRequirement class as policy
                services.AddAuthorization(opt => {
                    opt.AddPolicy("IsActivityHost", policy =>
                    {
                        policy.Requirements.Add(new IsHostRequirement());
                    });
                });
                //Makes this last as long as the method is running
                services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
                //Token service will be available when injected to account controler
                //This is scoped to the http request
                services.AddScoped<TokenService>();

                return services;
            }
    }
}