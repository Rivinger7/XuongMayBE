using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.AssemblyLineModelViews;

namespace XuongMayBE.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/assembly-lines")]
    [ApiController]
    public class AssemblyLineController : ControllerBase
    {
        private readonly IAssemblyLineService _assemblyLineService;

        public AssemblyLineController(IAssemblyLineService assemblyLineService)
        {
            _assemblyLineService = assemblyLineService;
        }

        /// <summary>
        /// Retrieves all active assembly lines
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>A list of all active assembly lines</returns>
        [HttpGet("active")]
        public async Task<IActionResult> GetAllAssemblyLines(int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveAssemblyLines = await _assemblyLineService.GetAllAssemblyLineAsync(pageNumber, pageSize);
                return Ok(retrieveAssemblyLines);
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
        /// Retrieves an assembly line by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The assembly line with the ID</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssemblyLineByID(int id)
        {
            try
            {
                var retrieveAssemblyLine = await _assemblyLineService.GetAssemblyLineByIDAsync(id);
                return Ok(retrieveAssemblyLine);
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
        /// Retrieves an assembly line by manager's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The assembly line with by manager's name</returns>
        [HttpGet("by-manager/{name}")]
        public async Task<IActionResult> GetAssemblyLineByManagerName(string name)
        {
            try
            {
                var retrieveAssemblyLine = await _assemblyLineService.GetAssemblyLineByManagerNameAsync(name);
                return Ok(retrieveAssemblyLine);
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
        /// Retrieves an assembly line by the manager's ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The assembly line managed by the manager's ID</returns>
        [HttpGet("by-manager/{id:int}")]
        public async Task<IActionResult> GetAssemblyLineByManagerIDAsync(int id)
        {
            try
            {
                var retrieveAssemblyLine = await _assemblyLineService.GetAssemblyLineByManagerIDAsync(id);
                return Ok(retrieveAssemblyLine);
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
        /// Retrieves assembly lines by the creator's name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>A list of assembly lines created by the creator's name</returns>
        [HttpGet("by-creator/{name}")]
        public async Task<IActionResult> GetAssemblyLinesByCreatorAsync(string name, int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveAssemblyLine = await _assemblyLineService.GetAssemblyLinesByCreatorAsync(name, pageNumber, pageSize);
                return Ok(retrieveAssemblyLine);
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
        /// Searches for assembly lines based on name and description filters
        /// </summary>
        /// <param name="assemblyLineName"></param>
        /// <param name="creator"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>A list of assembly lines matching the specified filters</returns>
        [HttpGet("search")]
        public async Task<IActionResult> GetAssemblyLinesByFilteringAsync([FromQuery] string? assemblyLineName, [FromQuery] string? creator, int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var retrieveAssemblyLine = await _assemblyLineService.GetAssemblyLinesByFilteringAsync(assemblyLineName, creator, pageNumber, pageSize);
                return Ok(retrieveAssemblyLine);
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
        /// Creates a new assembly line
        /// </summary>
        /// <param name="assemblyLineModel"></param>
        /// <returns>A confirmation message indicating successful creation</returns>
        [HttpPost("create-assembly-line")]
        public async Task<IActionResult> CreateAssemblyLineAsync(AssemblyLineCreateModel assemblyLineModel)
        {
            try
            {
                await _assemblyLineService.CreateAssemblyLineAsync(assemblyLineModel);
                return Ok(new { message = "Created an assembly line Successfully" });
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
        /// Updates an existing assembly line
        /// </summary>
        /// <param name="id"></param>
        /// <param name="assemblyLineModel"></param>
        /// <returns>A confirmation message indicating successful update</returns>
        [HttpPut("assembly-line")]
        public async Task<IActionResult> UpdateAssemblyLineAsync(int id, AssemblyLineUpdateModel assemblyLineModel)
        {
            try
            {
                await _assemblyLineService.UpdateAssemblyLineAsync(id, assemblyLineModel);
                return Ok(new { message = $"Updated assembly line with ID {id} Successfully" });
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
        /// Soft deletes an assembly line by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A confirmation message indicating successful deletion</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAssemblyLineByIDAsync(int id)
        {
            try
            {
                await _assemblyLineService.DeleteAssemblyLineByIDAsync(id);
                return Ok(new { message = $"Delete assembly line with ID {id} Successfully" });
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
