﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.AuthModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthencationService _authencationService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthencationService authencationService, IConfiguration configration)
        {
            _authencationService = authencationService;
            _configuration = configration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelView loginModel)
        {
            try
            {
                var userModel = await _authencationService.AuthenticateUserAsync(loginModel);

                Console.WriteLine(_configuration.GetSection("Jwt:SecretKey").Value);
                // JWT
                string token = JwtHelper.GenerateToken(_configuration.GetSection("Jwt:SecretKey").Value, userModel.Username,userModel.Role);

                return Ok(new { message = "Login Successfully", userModel, token });
            }
            catch(ArgumentException aex)
            {
                return BadRequest(new { message = aex.Message });
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelView registerModel)
        {
            try
            {
                string password = registerModel.Password;
                string confirmedPassword = registerModel.ConfirmedPassword;

                bool isConfirmedPassword = password == confirmedPassword;
                if (!isConfirmedPassword)
                {
                    throw new ArgumentException("Password and Confirmed Password does not matches");
                }

                await _authencationService.RegisterUserAsync(registerModel);

                return Ok(new { message = "Account created successfully" });
            }
            catch(ArgumentException aex)
            {
                if (aex.ParamName == "usernameExists")
                {
                    return Conflict(new { message = aex.Message });
                }
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
            
        }

    }
}
