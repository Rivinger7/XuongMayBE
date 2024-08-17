using GarmentFactory.Repository.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.AuthModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GarmentFactoryDBContext _context;
        private readonly IAuthencationService _authencationService;
        private readonly IJwtService _jwtService;

        public AuthController(GarmentFactoryDBContext context, IAuthencationService authencationService,IJwtService jwtService)
        {
            _context = context;
            _authencationService = authencationService;
			_jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelView loginModel)
        {
            try
            {
                var user = await _authencationService.AuthenticateUserAsync(loginModel);

                var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role)
				};                

                //Call method to generate access token
                _jwtService.GenerateAccessToken(claims, user.Id, out string accessToken, out string refreshToken);

                return Ok(new AuthenticatedResponseModelView
                        {  
                            AccessToken = accessToken,
							RefreshToken = refreshToken
                        }
                );
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error", stackError = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelView registerModel)
        {
            try
            {
                await _authencationService.RegisterUserAsync(registerModel);
                return Ok(new { message = "Account created successfully" });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error", stackError = ex.Message });
            }

        }

    }
}
