using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        //Commands don't return data like query
        //Return Unit to tell we're not returning anything
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        //Used to validate Activities coming in.
        //AbstractValidator interface can achieve this
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                //The rule checks if the Activity is not empty. The ActivityValidator defined these rules
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            //inject data context into this so we can persist changes
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            { 
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //This will access the Users from the DB context.
                //Don't need user manager to access it.
                var user = await _context.Users.FirstOrDefaultAsync(x =>
                    x.UserName == _userAccessor.GetUsername());

                var attendee = new ActivityAttendee{
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true
                };

                //Add new attendee to attendees table
                request.Activity.Attendees.Add(attendee);

                //This add the Activity in memory, NOT in databse yet. It just tracks the new Activity
                _context.Activities.Add(request.Activity);

                //This will save the activity to the databse
                //SaveChangeAsync counts how many changes are made
                //If more than 0, it will be true that there are changes
                var result = await _context.SaveChangesAsync() > 0;

                //Return 400 error if not found
                if (!result) return Result<Unit>.Failure("Failed to create activity");

                //This just lets the API controller know that everything above is finished.
                //Task<Unit> Has nothing, and is just waiting for the request 
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}