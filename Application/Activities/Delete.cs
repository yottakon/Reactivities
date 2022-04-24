using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        //IRequest returns Result of type Unit
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //If not request.Id is found, it would make activity null
                var activity = await _context.Activities.FindAsync(request.Id);
            
                //Check if activity is null. Returning null makes Result object null
                //In BaseApiControllers, it will check if activity is null, and return NotFound
                //if (activity == null) return null;

                //This removes it from memory first, and sends this info to ActivitiesController
                _context.Remove(activity);

                //More than 0 means a save change was made
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the activity");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}