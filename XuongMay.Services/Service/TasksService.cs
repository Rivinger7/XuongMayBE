using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="isCompleted"></param>
		/// <param name="orderId"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
		public async Task<BasePaginatedList<TasksGettingModel>> GetAllTaskAsync(int pageIndex, int pageSize, bool? isCompleted, int? orderId)
		{
			//write query to get all Tasks in DB
			IQueryable<Tasks> taskList = _unitOfWork.GetRepository<Tasks>().Entities
																		.Where(tl => tl.DeletedTime == null)
																		.OrderBy(tl => tl.Id);
			if (isCompleted.HasValue )
			{
				DateTime now = CoreHelper.SystemTimeNows;
				//nếu isCompleted = true -> các task đã hoàn thành
				//nếu isCompleted = false -> các task chưa hoàn thành
				taskList = isCompleted.Value ? taskList.Where(t => t.EndTime <= now) : taskList.Where (t => t.EndTime > now);
			}
			if (orderId.HasValue )
			{
				bool isExistOrder = await _unitOfWork.GetRepository<Order>().Entities.AnyAsync(o => o.Id == orderId);
				//if order is not exist in DB with orderId from parameter, throw exception
				if (!isExistOrder)
				{
					throw new ErrorException(StatusCodes.Status404NotFound,
											new ErrorDetail() { ErrorMessage = "The Order can not found!" });
				}
				taskList = taskList.Where(tl => tl.OrderId == orderId); //các task thuộc order có id đã nhập
			}

			//if there is no Task in DB, throw exception
			if (!await taskList.AnyAsync())
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = "No Task is stored in database" });
			}

			//get paginated Tasks list
			BasePaginatedList<Tasks> paginatedTasks = await _unitOfWork.GetRepository<Tasks>().GetPagging(taskList, pageIndex, pageSize);

			//if there is no Task in the current page, throw exception
			if (paginatedTasks.Items.Count == 0)
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = "No Task found for the current page" });
			}
			//convert TaskList to TaskGettingModel List
			BasePaginatedList<TasksGettingModel> tasksGettingModelList = _mapper.Map<BasePaginatedList<Tasks>, BasePaginatedList<TasksGettingModel>>(paginatedTasks);

			return tasksGettingModelList;
		}

		/// <summary>
		/// Get specific Task by given Id
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
		public async Task<TasksGettingModel> GetTaskByIdAsync(int taskId)
		{
			//get Task by taskId and if it null (not exist in DB) => throw exception
			Tasks task = await _unitOfWork.GetRepository<Tasks>().Entities.FirstOrDefaultAsync(t => t.Id == taskId && t.DeletedTime == null)
																		?? throw new ErrorException(StatusCodes.Status404NotFound,
																									new ErrorDetail() { ErrorMessage = "Cannot find any Task by this Id!" });
			// map the task to a tasksGetting model
			return _mapper.Map<Tasks, TasksGettingModel>(task);

		}

		/// <summary>
		/// Add a new Task with information from TasksCreateModel
		/// </summary>
		/// <param name="taskModel"></param>
		/// <returns></returns>
		public async Task AddNewTaskAsync (TasksUpdatingModel taskModel)
		{
			//parse start time in model (string) to DateTime
			DateTime startTimeTasksModel = DateTime.ParseExact(taskModel.StartTime, "HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture);

			//parse end time in model (string) to DateTime
			DateTime endTimeTasksModel = DateTime.ParseExact(taskModel.EndTime, "HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture);

			//get Order by OrderId in model
			var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(taskModel.OrderId) 
										 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																	new ErrorDetail() { ErrorMessage = "The Order can not found!"});

			//get Assembly Line by AssemblyLineId in model
			var assemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().GetByIdAsync(taskModel.AssemblyLineId)
												 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																			new ErrorDetail() { ErrorMessage = "The Assembly Line can not found!" });

			//get Task which having the same Assembly Line with Task in model and having time is overlapped
			var tasksInSameAssemblyLine = await _unitOfWork.GetRepository<Tasks>()
														   .Entities
														   .FirstOrDefaultAsync(t => t.AssemblyLineId == taskModel.AssemblyLineId && !t.DeletedTime.HasValue
																					&& ((startTimeTasksModel >= t.StartTime && startTimeTasksModel < t.EndTime)
																						|| (endTimeTasksModel > t.StartTime && endTimeTasksModel <= t.EndTime)));

			//get total quantity of all Tasks in an Order, except Task in model
			var sumOfQuantityInOtherTasks = await GetTotalQuantityOfOtherTasks(taskModel.OrderId, null);

			//check validate for Task in model
			CheckValidateTask(taskModel, order, tasksInSameAssemblyLine, sumOfQuantityInOtherTasks, startTimeTasksModel, endTimeTasksModel);

			//create a new Task with information from model
			Tasks task = new Tasks()
			{
				OrderId = taskModel.OrderId,
				AssemblyLineId = taskModel.AssemblyLineId,
				Title = taskModel.Title,
				Description = taskModel.Description,
				Quantity = taskModel.Quantity,
				StartTime = startTimeTasksModel,
				EndTime = endTimeTasksModel
			};

			//insert Task to DB and save
			await _unitOfWork.GetRepository<Tasks>().InsertAsync(task);
			await _unitOfWork.SaveAsync();
		}

		/// <summary>
		/// Update the Task with Id by information from TasksUpdatingModel
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="taskModel"></param>
		/// <returns></returns>
		/// <exception cref="ErrorException"></exception>
		public async Task UpdateTaskAsync (int taskId,TasksUpdatingModel taskModel)
		{
			//parse start time in model (string) to DateTime
			DateTime startTimeTasksModel = DateTime.ParseExact(taskModel.StartTime, "HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture);

			//parse end time in model (string) to DateTime
			DateTime endTimeTasksModel = DateTime.ParseExact(taskModel.EndTime, "HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture);

			//get Task by taskId and if it null (not exist in DB) => throw exception
			var task = await _unitOfWork.GetRepository<Tasks>().Entities.FirstOrDefaultAsync(t => t.Id == taskId && t.DeletedTime == null)
										 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																	new ErrorDetail() { ErrorMessage = "The Task can not found!" });

			//get Order by OrderId in model
			var order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(taskModel.OrderId)
										 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																	new ErrorDetail() { ErrorMessage = "The Order can not found!" });

			//get Assembly Line by AssemblyLineId in model
			var assemblyLine = await _unitOfWork.GetRepository<AssemblyLine>().GetByIdAsync(taskModel.AssemblyLineId)
												 ?? throw new ErrorException(StatusCodes.Status404NotFound,
																			new ErrorDetail() { ErrorMessage = "The Assembly Line can not found!" });

			//get Task which having the same Assembly Line with Task in model and having time is overlapped
			var tasksInOtherAssemblyLine = await _unitOfWork.GetRepository<Tasks>()
														   .Entities
														   .FirstOrDefaultAsync(t => t.AssemblyLineId == taskModel.AssemblyLineId && t.Id != taskId && !t.DeletedTime.HasValue
																					&& ((startTimeTasksModel >= t.StartTime && startTimeTasksModel < t.EndTime)
																						|| (endTimeTasksModel > t.StartTime && endTimeTasksModel <= t.EndTime)));

			//get total quantity of all Tasks in an Order, except Task in model
			var sumOfQuantityInOtherTasks = await GetTotalQuantityOfOtherTasks(taskModel.OrderId, taskId);

			//check validate for Task in model
			CheckValidateTask(taskModel, order, tasksInOtherAssemblyLine, sumOfQuantityInOtherTasks, startTimeTasksModel, endTimeTasksModel);

			//update Task with information from model
			task.OrderId = taskModel.OrderId;
			task.AssemblyLineId = taskModel.AssemblyLineId;
			task.Title = taskModel.Title;
			task.Description = taskModel.Description;
			task.Quantity = taskModel.Quantity;
			task.StartTime = startTimeTasksModel;
			task.EndTime = endTimeTasksModel;
			task.LastUpdatedTime = CoreHelper.SystemTimeNows;

			//update Task to DB and save
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
			var task = await _unitOfWork.GetRepository<Tasks>().GetByIdAsync(taskId)
														?? throw new ErrorException(StatusCodes.Status404NotFound,
														new ErrorDetail () { ErrorMessage = "Can not find Task by this Id!"});
			//throw exception if Task having deleted time
			if(task.DeletedTime.HasValue)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
													new ErrorDetail() { ErrorMessage = "This task does not exist!" });
			}

			//if the start time less than time call this method, not allow to execute 
			if (task.StartTime < CoreHelper.SystemTimeNows)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
													new ErrorDetail() { ErrorMessage = "This task has started, can not be deleted!" });
			}
			//set deleted time is now
			task.DeletedTime = CoreHelper.SystemTimeNows;
			//set atatus deleted be true
			task.IsDeleted = true;

			//delete a Task and save
			await _unitOfWork.GetRepository<Tasks>().UpdateAsync(task);
			await _unitOfWork.SaveAsync();
		}


		private async Task<int> GetTotalQuantityOfOtherTasks(int orderId, int? taskIdUpdate)
		{
			//Sum of quantity of all Tasks in an Order
			return await _unitOfWork.GetRepository<Tasks>().Entities
														.Where(t => t.OrderId == orderId && t.Id != taskIdUpdate && !t.DeletedTime.HasValue)
														.SumAsync(t => t.Quantity);
		}

		private void CheckValidateTask(TasksUpdatingModel taskModel ,Order order, Tasks tasksInSameAssemblyLine, int sumOfQuantityInOtherTasks, DateTime startTimeTasksModel, DateTime endTimeTasksModel)
		{
			// Trường hợp: thời gian bắt đầu Task được gán trước thời gian bắt đầu của Order
			if (startTimeTasksModel < order.StartTime)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The Start Time must be after the Order Start Time!" });
			}

			// Trường hợp: thời gian bắt đầu Task sau thời gian kết thúc
			if (startTimeTasksModel > endTimeTasksModel)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The End Time must be greater than Start Time" });
			}

			// Trường hợp: thời gian bắt đầu hoặc thời gian kết thúc vượt quá deadline của order
			if (startTimeTasksModel >= order.EndTime || endTimeTasksModel >= order.EndTime)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "Exceeding the deadline time allowed in the Order" });
			}

			// Trường hợp: Task này có thời gian trùng hoặc nằm trong một Task khác trong cùng một dây chuyền (assembly line)
			if (tasksInSameAssemblyLine != null)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "This Task has time incuding in other Task which having same assembly line" });
			}

			if (string.IsNullOrWhiteSpace(taskModel.Title))
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The title must have at least 1 character!" });
			}

			// Trường hợp: số lượng của Task này vượt quá số lượng của Order
			if (taskModel.Quantity <= 0)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "Please enter quantity greater than 0!" });
			}

			// Trường hợp: tổng số lượng của các Task khác và Task này của một Order cụ thể vượt quá số lượng của Order đó
			if (sumOfQuantityInOtherTasks + taskModel.Quantity > order.Quantity)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "The quantity exceeds order quantity limit!" });
			}
		}

	}
}
