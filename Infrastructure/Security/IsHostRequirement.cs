using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private DataContext _dbContext;

        //Data context is used to connect to the database
        public IsHostRequirementHandler(DataContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;

        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            //get user ID from authorization handler
            //Find name idntifier to get user id
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //The user is not authorized to access
            if (userId == null) return Task.CompletedTask;

            //Gets activityId value from route parameters
            //Gets http request to get route values
            //Guid.Parse method needs a string
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString());

            //Uses userId and activityId to find attendee
            //Result gets result of the query
            //Tracking will keep the info in memory, which causes problems. Us AsNoTracking to fix
            var attendee = _dbContext.ActivityAttendees
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId)
                .Result;

            //if no attendee
            if (attendee == null) return Task.CompletedTask;

            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}