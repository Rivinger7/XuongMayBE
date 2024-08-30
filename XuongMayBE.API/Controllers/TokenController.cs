using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.JwtModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMayBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TokenController : Controller
	{
		private readonly IJwtService _jwtService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TokenController(IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
		{
			_jwtService = jwtService;
			_httpContextAccessor = httpContextAccessor;
		}

		/// <summary>
		/// Refresh the token when expired
		/// </summary>
		/// <param name="tokenApiModel"></param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshAccessToken(TokenApiModelView tokenApiModel)
		{
			try
			{
				//Call method to refresh access token and get 2 results newAccessToken and newRefreshToken
				_jwtService.RefreshAccessToken(out string newAccessToken, out string newRefreshToken, tokenApiModel);
				return Ok(new AuthenticatedResponseModelView()
				{
					AccessToken = newAccessToken,
					RefreshToken = newRefreshToken
				});
			}
			catch (ErrorException eex)
			{
				return StatusCode(eex.StatusCode, eex.ErrorDetail);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}


		/// <summary>
		/// Revoke the token when neccessary if have any problem about security,...
		/// </summary>
		/// <returns></returns>
		[HttpPost("revoke")]
		public async Task<IActionResult> Revoke()
		{
			try
			{
				//var Id = User.Identity.Name; //get username from claim in token
				string userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				_jwtService.RevokeToken(userId);
				return NoContent();
			}
			catch (ErrorException eex)
			{
				return StatusCode(eex.StatusCode, eex.ErrorDetail);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

	}
}
