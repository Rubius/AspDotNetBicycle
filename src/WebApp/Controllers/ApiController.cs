using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize]
[ApiController]
public abstract class ApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator
    {
        get
        {
            if (_mediator == null)
            {
                var result = HttpContext.RequestServices.GetService<IMediator>();

                _mediator = result ?? throw new Exception("Cannot instantiate the Mediator");
            }
                    
            return _mediator;
        }
    }
}
