using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        //This class will fetch and return activity data
        public class Query : IRequest<Result<ActivityDto>>
        {
            //Specify the activty ID to retrieve
            public Guid Id {get; set; }
        }

        //Access Id using a handler
        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            //Create constructor to inject DataContext
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                //Returns the activity based on the ID requested.
                //Request is the query class above
                //If it can't an activity, it returns null.
                //Project to mapper is used to auotmatically map the Activity Dto base don activity
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                //Result Success function returns the activity if it is found
                return Result<ActivityDto>.Success(activity);
            }
        }
    }
}