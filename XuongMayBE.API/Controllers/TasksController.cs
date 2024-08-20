using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.TasksModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMayBE.API.Controllers
{
	//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
	[Route("api/[controller]")]
	[ApiController]
	public class TasksController : ControllerBase
	{
		private readonly ITasksService _taskService;

		public TasksController(ITasksService taskService) //inject Task service
		{
			_taskService = taskService;
		}

		/// <summary>
		/// Find a Task by given Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A correlative Task</returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				//Call method to check get a task
				TasksGettingModel task = await _taskService.GetTaskByIdAsync(id);
				return Ok(task);
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception ex)
			{
				var log = new LoggerConfiguration()
								.WriteTo.Console(theme: AnsiConsoleTheme.Literate)
								.CreateLogger();
				log.Error($"{ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		/// <summary>
		/// Find all Task which were stored
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="isCompleted"></param>
		/// <param name="orderId"></param>
		/// <returns>The amount of Task with pageSize and pageIndex</returns>
		[HttpGet]
		public async Task<IActionResult> GetTasks(int pageIndex = 1, int pageSize = 10, bool? isCompleted = null, int? orderId = null)
		{
			try
			{
				//Call method and then return list of available tasks
				var taskList = await _taskService.GetAllTaskAsync(pageIndex, pageSize, isCompleted, orderId);
				return Ok(taskList.Items);
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex.StackTrace);
			//	return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			//}
		}

		/// <summary>
		/// Add a new Task by give necessary information in model
		/// </summary>
		/// <param name="taskModel"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Add(TasksUpdatingModel taskModel)
		{
			try
			{
				await _taskService.AddNewTaskAsync(taskModel);
				return Ok("Add successfully!");
			}
			catch (ErrorException ex)
			{
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		/// <summary>
		/// Unable a Task and set time delete to it
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Status 200 if success</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				//Call method to check and delete
				await _taskService.DeleteTaskAsync(id);
				return Ok("Delete successfully!");
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, TasksUpdatingModel taskModel)
		{
			try
			{
				//Call method to check and delete
				await _taskService.UpdateTaskAsync(id, taskModel);
				return Ok("Update successfully!");
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

	}
}
