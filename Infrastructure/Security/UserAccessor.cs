using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Security
{
	public class UserAccessor : IUserAccessor
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserAccessor(IHttpContextAccessor httpContext) { 
			_httpContextAccessor = httpContext;
		
		}
		public string GetUsername()
		{
			return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
		}
	}
}
