using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
    //this will list activities
    public class List
    {
        //Nested class
        public class Query : IRequest<Result<List<ActivityDto>>> { }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            //private readonly ILogger<List> _logger;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;

                //If ILogger<List> logger was put in parameter, it would be used to catch logs when used, such as cancellation token sent
                //_logger = logger;
            }

            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //Below would be used if you wanted to pass CancellationToken to cancel a request. This won't be used
                /*
                try
                {
                     for (var i = 0; i<10; i++){
                         cancellationToken.ThrowIfCancellationRequested();
                         await Task.Delay(1000, cancellationToken);
                         _logger.LogInformation($"Task {i} has completed");
                     }
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    //If cancellation is requestion, send logger information
                    _logger.LogInformation("Task was cancelled");
                    throw;
                }
                return await _context.Activities.ToListAsync(cancellationToken);
                */

                //Attendees is the join table. Use this table to get users
                //ThenInclude will get the related user entity to the attendees
                //Projects to ActivtyDto. This automativally makes activities an ActivityDto list. 
                //Automapper simplifies ths table joining
                var activities = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                //Using Automapper, or IMapper _mapper, this will automatically return a list of ActivityDtos from the list of activities
                //var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);

                //Getting activities for a user to a list and returning them
                return Result<List<ActivityDto>>.Success(activities);
            }

        }
    }
}