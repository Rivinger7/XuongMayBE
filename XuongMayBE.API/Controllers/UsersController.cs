using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;

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

        /// <summary>
        /// Retrieves a list of all users
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// An IActionResult containing a list of all users
        /// Returns a BadRequest if there is an issue with the request or if no managers are found
        /// Returns an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveUsers = await _userService.GetAllUsersAsync(pageNumber, pageSize);
                return Ok(retrieveUsers);
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
        /// Retrieves a list of all users with the role of 'Admin'
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// An IActionResult containing a list of all admins
        /// Returns a BadRequest if there is an issue with the request or if no managers are found
        /// Returns an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdmins(int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveUsers = await _userService.GetAllAdminsAsync(pageNumber, pageSize);
                return Ok(retrieveUsers);
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
        /// Retrieves a list of all users with the role of 'Manager'
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// An IActionResult containing a list of all managers
        /// Returns a BadRequest if there is an issue with the request or if no managers are found
        /// Returns an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("manager")]
        public async Task<IActionResult> GetAllManagers(int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveUsers = await _userService.GetAllManagersAsync(pageNumber, pageSize);
                return Ok(retrieveUsers);
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
        /// Retrieves a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// An IActionResult containing the user data if found
        /// Returns a BadRequest if the provided id is invalid or the user does not exist
        /// Returns an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserByID(int id)
        {
            try
            {
                var retrieveUser = await _userService.GetUserByIDAsync(id);
                return Ok(retrieveUser);
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
        /// Searches for users based on the specified filtering criteria: username, full name, and role
        /// </summary>
        /// <param name="username"></param>
        /// <param name="fullName"></param>
        /// <param name="role"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// An IActionResult containing a list of users that match the specified criteria
        /// Returns a BadRequest if the input parameters are invalid, or an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? username = null, [FromQuery] string? fullName = null, [FromQuery] string? role = null, int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveUsers = await _userService.GetUsersByFilteringAsync(username, fullName, role, pageNumber, pageSize);
                return Ok(retrieveUsers);
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
        /// Changes a user's password by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        /// <param name="newConfirmPassword"></param>
        /// <returns>
        /// An IActionResult indicating the result of the password change operation
        /// Returns an Ok response if the password is changed successfully
        /// Returns a BadRequest if the provided input is invalid or the password change fails
        /// Returns an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager,Admin")]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword(int id, string newPassword, string newConfirmPassword)
        {
            try
            {
                await _userService.UpdatePasswordAsync(id, newPassword, newConfirmPassword);

                return Ok(new { message = "Chanage Password Successfully" });
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
        /// Changes a user's full name by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newFullName"></param>
        /// <returns>
        /// An IActionResult indicating the result of the full name change operation
        /// Returns an Ok response if the full name is changed successfully
        /// Returns a BadRequest if the provided input is invalid or the full name change fails
        /// Returns an Internal Server Error if an unexpected error occurs
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager,Admin")]
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
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error", stackError = ex.Message });
            }
        }

        /// <summary>
        /// Soft Deletes a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// An IActionResult indicating the result of the soft delete operation.
        /// Returns an Ok response if the user is soft deleted successfully.
        /// Returns a BadRequest if the provided id is invalid or the user could not be found.
        /// Returns an Internal Server Error if an unexpected error occurs. 
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
                return BadRequest(new { message = aex.Message });
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error", stackError = ex.Message });
            }
        }

        /// <summary>
        /// Soft Deletes a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>
        /// An IActionResult indicating the result of the soft delete operation.
        /// Returns an Ok response if the user is soft deleted successfully.
        /// Returns a BadRequest if the provided username is invalid or the user could not be found.
        /// Returns an Internal Server Error if an unexpected error occurs. 
        /// </returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
