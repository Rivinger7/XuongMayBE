using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.TasksModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMay.Services.Service
{
	public class TasksService : ITasksService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TasksService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork; 
		}

		/// <summary>
		/// Show all Tasks which having in DB
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
		public async Task<IList<Tasks>> GetAllTaskAsync()
		{
			//get Tasks with Id
			IList<Tasks> taskList = await _unitOfWork.GetRepository<Tasks>().GetAllAsync();

			//check weather taskList exist or not
			if (!taskList.Where(t => t.DeletedTime == null).Any())
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = "No Task is stored in database" });
			}
			return taskList;

		}

		public async Task<Tasks> GetTaskByIdAsync(int taskId)
		{
			Tasks task = await _unitOfWork.GetRepository<Tasks>().GetByIdAsync(taskId)
								?? throw new ErrorException(StatusCodes.Status404NotFound,
															new ErrorDetail() { ErrorMessage = "Can not find any Task by this Id!" });

			if(task.DeletedTime != null)
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
															new ErrorDetail() { ErrorMessage = "Can not find any Task by this Id!" });
			}

			return task;
		}


		/// <summary>
		/// Add a new Task with information from TasksCreateModel
		/// </summary>
		/// <param name="taskModel"></param>
		/// <returns></returns>
		public async Task AddNewTaskAsync (TasksCreateModel taskModel)
		{

		}

		/// <summary>
		/// Delete a Task with Id
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
		public async Task DeleteTaskAsync (int taskId)
		{
			//get Task by taskId and if it null (not exist in DB) => throw exception
			var task = _unitOfWork.GetRepository<Tasks>().GetById(taskId)
														?? throw new ErrorException(StatusCodes.Status404NotFound,
														new ErrorDetail () { ErrorMessage = "Can not find Task by this Id!"});
			if(task.DeletedTime != null)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
														new ErrorDetail() { ErrorMessage = "This task is not exist!" });
			}
			//set deleted time is now
			task.DeletedTime = CoreHelper.SystemTimeNows;
			//set atatus deleted be true
			task.IsDeleted = true;

			//delete a Task and save
			await _unitOfWork.GetRepository<Tasks>().UpdateAsync(task);
			await _unitOfWork.SaveAsync();
		}

	}
}
