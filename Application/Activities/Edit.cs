using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

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
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper){
                _context = context;
                _mapper = mapper;
                //mapper is the Automapper
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //need to get activity from database, and fill in with the activity we want to add
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                //If Activity.Title is null, it will set it to activity.Title
                //It might be null if the user makes a request without a title
                //activity.Title = request.Activity.Title ?? activity.Title;

                if (activity == null) return null;

                //Automapper allows it to map the Activity the user requests to the activity from the database 
                //This allows the user to update all the fields in Activity, instead of typing out all the fields to check
                _mapper.Map(request.Activity, activity);

                var result = await _context.SaveChangesAsync() >0 ;

                if (!result) return Result<Unit>.Failure("Failed to update activity");

                //It will say it was sucessful
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}