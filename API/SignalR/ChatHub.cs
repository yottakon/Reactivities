using System;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        //client can make a connection to this Hub, and invoke methods
        public async Task SendComment(Create.Command command)
        {
            //send the properties from Create, which are the activity ID and body of the comment
            var comment = await _mediator.Send(command);

            //name of method ReceiveComment used in client side to get the comment
            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);
        }

        //Signal R will automatically remove connection ID when a client disconnects from SignalR
        //Whenever someone connects, they will join the group based on activity ID
        //It will then send the list of comments from the database.
        public override async Task OnConnectedAsync()
        {
            //get activty id from query string to send out information
            var httpContext = Context.GetHttpContext();
            //Get key activityId 
            var activityId = httpContext.Request.Query["activityId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            //Mediator will send a list query, and return the result
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)});
            //This returns comments to ust the Caller who requested it
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}