using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BaseApiController : ControllerBase
	{
		private readonly IMediator _mediator;

		protected IMediator Mediator => _mediator ?? HttpContext.RequestServices.GetService<IMediator>();


		protected ActionResult HandleResult<T>(Result<T> result)
		{
			if(result== null) { return NotFound(); }
			if (result.Value != null && result.IsSuccess)
			{
				return Ok(result.Value);
			}
			if (result.Value == null && result.IsSuccess)
			{
				return NotFound();
			}
			return BadRequest(result.Error);
		}
	}
}
