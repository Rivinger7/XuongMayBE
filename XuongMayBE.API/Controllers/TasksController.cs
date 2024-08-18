using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.TasksModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMayBE.API.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
	[Route("api/[controller]")]
	[ApiController]
	public class TasksController : ControllerBase
	{
		private readonly ITasksService _taskService;

		public TasksController(ITasksService taskService) //inject Task service
		{
			_taskService = taskService;
		}

		[HttpGet("get_by_id")]
		public async Task<IActionResult> Get(int taskId)
		{
			try
			{
				//Call method to check and delete
				Tasks task = await _taskService.GetTaskByIdAsync(taskId);
				return Ok(task);
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		[HttpGet("get_all")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				//Call method to check and delete
				IList<Tasks> taskList = await _taskService.GetAllTaskAsync();
				return Ok(taskList);
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		[HttpPost("add")]
		public async Task<IActionResult> Add(TasksCreateModel taskModel)
		{
			return Ok();
		}

		[HttpDelete("delete/{taskId}")]
		public async Task<IActionResult> Delete(int taskId)
		{
			try
			{
				//Call method to check and delete
				await _taskService.DeleteTaskAsync(taskId);
				return Ok("Delete successfully!");
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		[HttpPut("update/{taskId}")]
		public async Task<IActionResult> Update(int taskId)
		{
			return Ok();
		}
	}
}
