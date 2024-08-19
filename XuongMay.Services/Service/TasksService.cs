using AutoMapper;
using GarmentFactory.Repository.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

			if (paginatedTasks.Items.Count == 0)
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = "No Task found for the current page" });
			}

			return paginatedTasks;
		}

		/// <summary>
		/// Get specific Task by given Id
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
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
		public async Task AddNewTaskAsync (TasksGeneralModel taskModel)
		{
			var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(taskModel.OrderId) 
										 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																	new ErrorDetail() { ErrorMessage = "The Order can not found!"});

			var assemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().GetByIdAsync(taskModel.AssemblyLineId)
												 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																			new ErrorDetail() { ErrorMessage = "The Assembly Line can not found!" });

			var tasksInSameAssemblyLine = await _unitOfWork.GetRepository<Tasks>()
														   .Entities
														   .FirstOrDefaultAsync(t => t.AssemblyLineId == taskModel.AssemblyLineId 
																					&& taskModel.StartTime >= t.StartTime 
																					&& taskModel.EndTime <= t.EndTime);

			var sumOfQuantityInOtherTasks = await GetTotalQuantityOfTasks(taskModel.OrderId);

			CheckValidateTask(taskModel, order, assemblyLine, tasksInSameAssemblyLine, sumOfQuantityInOtherTasks);

			Tasks task = new Tasks()
			{
				OrderId = taskModel.OrderId,
				AssemblyLineId = taskModel.AssemblyLineId,
				Title = taskModel.Title,
				Description = taskModel.Description,
				Quantity = taskModel.Quantity,
				StartTime = taskModel.StartTime,
				EndTime = taskModel.EndTime
			};

			await _unitOfWork.GetRepository<Tasks>().InsertAsync(task);
			await _unitOfWork.SaveAsync();
		}

		public async Task UpdateTaskAsync (int taskId,TasksGeneralModel taskModel)
		{
			var task = await _unitOfWork.GetRepository<Tasks>().GetByIdAsync(taskId)
										 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																	new ErrorDetail() { ErrorMessage = "The Task can not found!" });

			var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(taskModel.OrderId)
										 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																	new ErrorDetail() { ErrorMessage = "The Order can not found!" });

			var assemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().GetByIdAsync(taskModel.AssemblyLineId)
												 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																			new ErrorDetail() { ErrorMessage = "The Assembly Line can not found!" });

			var tasksInSameAssemblyLine = await _unitOfWork.GetRepository<Tasks>()
														   .Entities
														   .FirstOrDefaultAsync(t => t.AssemblyLineId == taskModel.AssemblyLineId
																					&& taskModel.StartTime >= t.StartTime
																					&& taskModel.EndTime <= t.EndTime);

			var sumOfQuantityInOtherTasks = await GetTotalQuantityOfTasks(taskModel.OrderId);

			CheckValidateTask(taskModel, order, assemblyLine, tasksInSameAssemblyLine, sumOfQuantityInOtherTasks);

			task.OrderId = taskModel.OrderId;
			task.AssemblyLineId = taskModel.AssemblyLineId;
			task.Title = taskModel.Title;
			task.Description = taskModel.Description;
			task.Quantity = taskModel.Quantity;
			task.StartTime = taskModel.StartTime;
			task.EndTime = taskModel.EndTime;
			task.LastUpdatedTime = CoreHelper.SystemTimeNows;

			await _unitOfWork.GetRepository<Tasks>().UpdateAsync(task);
			await _unitOfWork.SaveAsync();
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



		private async Task<int> GetTotalQuantityOfTasks(int orderId)
		{
			return await _unitOfWork.GetRepository<Tasks>().Entities
														.Where(t => t.OrderId == orderId)
														.SumAsync(t => t.Quantity);
		}

		private void CheckValidateTask(TasksGeneralModel taskModel ,Order order, AssemblyLine assemblyLine, Tasks tasksInSameAssemblyLine, int sumOfQuantityInOtherTasks)
		{
			if (taskModel.StartTime < order.StartTime)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The Start Time must be after the Order Start Time!" });
			}

			if (taskModel.StartTime > taskModel.EndTime)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The End Time must be greater than Start Time" });
			}

			if (taskModel.StartTime >= order.EndTime || taskModel.EndTime >= order.EndTime)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "Exceeding the deadline time allowed in the Order" });
			}

			if (tasksInSameAssemblyLine != null)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "This Task has time incuding in other Task which having same assembly line" });
			}

			if (sumOfQuantityInOtherTasks + taskModel.Quantity > order.Quantity)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The quantity exceeds order quantity limit!" });
			}
		}

	}
}
