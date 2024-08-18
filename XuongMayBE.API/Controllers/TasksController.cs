using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
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

		[HttpGet("get_by_id/{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				//Call method to check get a task
				TasksModel task = await _taskService.GetTaskByIdAsync(id);
				return Ok(task);
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		[HttpGet("get_all")]
		public async Task<IActionResult> GetAll(int pageIndex, int pageSize)
		{
			try
			{
				//Call method and then return list of available tasks
				var taskList = await _taskService.GetAllTaskAsync(pageIndex, pageSize);
				return Ok(taskList.Items);
			}
			catch (ErrorException ex)
			{
				// Return the status code and message from the ErrorException
				return StatusCode(ex.StatusCode, ex.ErrorDetail);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		[HttpPost("add")]
		public async Task<IActionResult> Add(TasksCreateModel taskModel)
		{
			return Ok();
		}

		[HttpDelete("delete/{id}")]
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
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "An server error occurred while processing your request.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		[HttpPut("update/{taskId}")]
		public async Task<IActionResult> Update(int taskId)
		{
			return Ok();
		}
	}
}
