using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    //IHostEnvironment tells whether something is in development node or not
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        //every middleware is a request delegate
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Whenever a request comes in, it will go to the exception middleware, and then the next exception middleware (if there are no issues caught here)
            //If it gets caught here, then we can see the exception here
            try {
                await _next(context);
            }
            catch (Exception ex)
            {
                //Goes to normal logging window to see what happens
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                //Status code is set to 500

                //checks if in development mode
                //If it is, then send back the full exception with stack trace
                //The StackTrace? is optional chaining
                //Else, it returns a generic server error
                var response = _env.IsDevelopment()
                    ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new AppException(context.Response.StatusCode, "Server Error");

                //Set naming policy to ensure it is CamelCase and not titlecase
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}