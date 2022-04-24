using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Application.Activities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    //Allow Anonymous will let anyone view a page without authentication
    //[AllowAnonymous]
    public class ActivitiesController : BaseApiController
    {
        //The Database context is based on the Database you are connecting to
        //Changed to use Mediator instead of DataContext
        //Deleted, and moved to BaseApiController
        

        //returns a list of activities from the database
        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            //Since this class is derived by BaseApiController, it has a Mediator inject
            //Mediator relate to flow of control in the application. API controllers will send an object to Mediator
            //Mediator has a handler that will process business logic
            //Object will be sent back to API controller, which will send back to client as HTTP response.
            return HandleResult(await Mediator.Send(new List.Query()));

            //If CancellationToke ct is put as a parameter in GetActivities, then the Handler will check when a request is cancelled
            //return await Mediator.Send(new List.Query(ct));
        }

        //This will return a single activity based on the ID
        [HttpGet("{id}")] //activities/id
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            //return await _context.Activities.FindAsync(id);
            //creates a new instance of Details using an Id set by the Guid id
            //HandleResult from BaseApiController
            return HandleResult(await Mediator.Send(new Details.Query{Id = id}));
        }

        //IActionResult returns if a request is OK, or bad request, return, or return not found
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            //This will look inside the body of hte Activity object, and see if it really is an Activity
            return HandleResult(await Mediator.Send(new Create.Command {Activity = activity}));
        }

        //Put request that requires an activity Id
        //The Authorize Policy IsActivityHost was creates in IsHostRequirement file
        //This makes only a host of an activity to edit
        [Authorize(Policy = "IsActivityHost")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            //this will add the new Guid to the activity before it is sent to the Handler
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command{Activity = activity}));
        }

        //The Authorize Policy IsActivityHost was creates in IsHostRequirement file
        //This makes only a host of an activity to delete
        [Authorize(Policy = "IsActivityHost")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            //This will set the Id when it instantiates the Command class
            return HandleResult(await Mediator.Send(new Delete.Command{Id = id}));
        }

        //This handles both attending and removing from an activity
        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command{Id = id}));
        }

    }
}