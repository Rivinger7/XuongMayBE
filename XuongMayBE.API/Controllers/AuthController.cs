﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AuthModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthencationService _authencationService;

        public AuthController(IAuthencationService authencationService)
        {
            _authencationService = authencationService;
        }

        /// <summary>
        /// Authenticates a user based on their login credentials
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>An access token and a refresh token if authentication is successful</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelView loginModel)
        {
            try
            {
                var authenticationModel = await _authencationService.AuthenticateUserAsync(loginModel);
                return Ok(authenticationModel);
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

        /// <summary>
        /// Registers a new user account (Manager)
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns>A confirmation message indicating successful account creation</returns>
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
