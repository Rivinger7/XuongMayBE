using GarmentFactory.Repository.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.AuthModelViews;
using XuongMay.ModelViews.JwtModelViews;

namespace XuongMayBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class TokenController : Controller
	{
		private readonly IJwtService _jwtService;

		public TokenController(IJwtService jwtService)
		{
			_jwtService = jwtService;
		}

		/// <summary>
		/// Refresh the token when expired
		/// </summary>
		/// <param name="tokenApiModel"></param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpPost("refresh_token")]
		public async Task<IActionResult> RefreshAccessToken(TokenApiModelView tokenApiModel)
		{
			//Call method to refresh access token and get 2 results newAccessToken and newRefreshToken
			_jwtService.RefreshAccessToken(out string newAccessToken, out string newRefreshToken, tokenApiModel);
			return Ok(new AuthenticatedResponseModelView()
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken
			});
		}


		/// <summary>
		/// Revoke the token when neccessary if have any problem about security,...
		/// </summary>
		/// <returns></returns>
		[HttpPost("revoke")]
		public async Task<IActionResult> Revoke()
		{
			var Id = User.Identity.Name; //get username from claim in token
			_jwtService.RevokeToken(Id);
			return NoContent();
		}

	}
}
