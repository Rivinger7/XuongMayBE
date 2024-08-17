using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;

namespace XuongMayBE.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return users.Any() ? Ok(new { message = $"Found {users.Count()}", users }) : NotFound("Not Found!!!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var users = await _userService.GetAllAdminsAsync();
                return users.Any() ? Ok(new { message = $"Found {users.Count()}", users }) : NotFound("Not Found!!!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        [HttpGet("manager")]
        public async Task<IActionResult> GetAllManagers()
        {
            try
            {
                var users = await _userService.GetAllManagersAsync();
                return users.Any() ? Ok(new { message = $"Found {users.Count()}", users }) : NotFound("Not Found!!!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }

        }

        [HttpGet("{id:int}")]
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? username = null, [FromQuery] string? fullName = null, [FromQuery] string? role = null)
        {
            try
            {
                var users = await _userService.GetUsersAsync(username, fullName, role);
                return Ok(users);
            }
            catch(ArgumentException aex)
            {
                return NotFound(aex.Message);
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

        [HttpDelete("{id:int}")]
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

        [HttpDelete("{username}")]
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
