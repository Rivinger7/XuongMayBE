using AutoMapper;
using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection.Metadata.Ecma335;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.TasksModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMay.Services.Service
{
	public class TasksService : ITasksService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public TasksService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork; 
			_mapper = mapper;
		}

		/// <summary>
		/// Show all Tasks which having in DB
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
		//public async Task<BasePaginatedList<Tasks>> GetAllTaskAsync(int pageIndex, int pageSize)
		//{
		//	//get Tasks with Id
		//	IQueryable<Tasks> taskList = _unitOfWork.GetRepository<Tasks>()
		//											.GetAll()
		//											.AsQueryable();
		//											//.Where(tl => tl.DeletedTime == null)
		//											//.OrderBy(tl => tl.Id); ;


		//	//check weather taskList exist or not
		//	if (!taskList.Any())
		//	{
		//		throw new ErrorException(StatusCodes.Status404NotFound,
		//								new ErrorDetail() { ErrorMessage = "No Task is stored in database" });
		//	}
		//	// Get paginated tasks
		//	var paginatedTasks = await _unitOfWork.GetRepository<Tasks>().GetPagging(taskList, pageIndex, pageSize);

		//	return paginatedTasks;

		//}

		public async Task<BasePaginatedList<Tasks>> GetAllTaskAsync(int pageIndex, int pageSize)
		{
			IQueryable<Tasks> taskList = _unitOfWork.GetRepository<Tasks>().Entities
																		.Where(tl => tl.DeletedTime == null)
																		.OrderBy(tl => tl.Id);

			if (!await taskList.AnyAsync())
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = "No Task is stored in database" });
			}

			var paginatedTasks = await _unitOfWork.GetRepository<Tasks>().GetPagging(taskList, pageIndex, pageSize);

			//if (paginatedTasks.Items.Count == 0)
			//{
			//	throw new ErrorException(StatusCodes.Status404NotFound,
			//							new ErrorDetail() { ErrorMessage = "No Task found for the current page" });
			//}

			return paginatedTasks; //_mapper.Map<BasePaginatedList<Tasks>, BasePaginatedList<TasksModel>>(paginatedTasks);
		}

		public async Task<TasksModel> GetTaskByIdAsync(int taskId)
		{
			var task = await _unitOfWork.GetRepository<Tasks>().GetByIdAsync(taskId);

			// Check if task is null or deleted
			if (task == null || task.DeletedTime != null)
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = "Cannot find any Task by this Id!" });
			}

			// Map the task to a model (if needed)
			TasksModel tasksModel = _mapper.Map<Tasks, TasksModel>(task);

			return tasksModel;
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
