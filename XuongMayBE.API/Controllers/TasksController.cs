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

		/// <summary>
		/// Find all Task which were stored
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <returns>The amount of Task with pageSize and pageIndex</returns>
		[HttpGet("get_all")]
		public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
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
		[HttpPost("add")]
		public async Task<IActionResult> Add(TasksGeneralModel taskModel)
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
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, TasksGeneralModel taskModel)
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
