using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    //this will add the ApiController attribute
    //The controller will be whatever controller you want to use. After api/write down the name of the controller, without the controller part, in the browser
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        //Any controller created will have mediator immediately injected
        private IMediator _mediator;

        //protected means it can be used by derived classes and baseApiController itself
        //The ??= means if _mediator is null, it will assign Mediator to whatever is on the right of ??=
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices
            .GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> result){
            if(result == null) return NotFound();
            if (result.IsSuccess && result.Value != null){
                return Ok(result.Value);
            }
            if (result.IsSuccess && result.Value == null){
                return NotFound();
            }

            //If neither null or activity found, result error
            return BadRequest(result.Error);
        }
        
    }
}