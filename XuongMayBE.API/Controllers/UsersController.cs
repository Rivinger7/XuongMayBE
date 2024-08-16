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
        public UsersController(IUserService userService) {
            _userService = userService;
        }
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return users is not null ? Ok(users) : NotFound("Not Found!!!");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                return StatusCode(500, new { message = "Internal server error" });
            }
            
        }
    }
}
