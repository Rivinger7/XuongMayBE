using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Base;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMayBE.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return users is not null ? Ok(new { message = $"Found {users.Count()}", users }) : NotFound("Not Found!!!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        [HttpPost("id")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            try
            {
                var user = await _userService.GetUserByIDAsync(id);
                return Ok(user);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("username")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                return user is not null ? Ok(user) : NotFound("Not Found!!!");
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("full-name")]
        public async Task<IActionResult> GetUserByFullName(string fullName)
        {
            try
            {
                var user = await _userService.GetUserByFullNameAsync(fullName);
                return Ok(user);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("role")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword(int id, string newPassword, string newConfirmPassword)
        {
            try
            {
                if (newPassword != newConfirmPassword)
                {
                    return BadRequest(new { message = "New password and new confirm password does not matches" });
                }

                await _userService.UpdatePasswordAsync(id, newPassword);

                return Ok(new { message = "Chanage Password Successfully" });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("full-name")]
        public async Task<IActionResult> ChangeFullName(int id, string newFullName)
        {
            try
            {
                await _userService.UpdateFullNameAsync(id, newFullName);

                return Ok(new { message = "Chanage Full Name Successfully" });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteUserByID(int id)
        {
            try
            {
                await _userService.DeleteUserByIDAsync(id);

                return Ok(new { message = "Delete User Successfully" });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("username")]
        public async Task<IActionResult> DeleteUserByUsername(string username)
        {
            try
            {
                await _userService.DeleteUserByUsernameAsync(username);

                return Ok(new { message = "Delete User Successfully" });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
