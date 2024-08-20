using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.TasksModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface ITasksService
	{
		Task<BasePaginatedList<TasksGettingModel>> GetAllTaskAsync(int pageIndex, int pageSize, bool? isCompleted, int? orderId);
		Task<TasksGettingModel> GetTaskByIdAsync(int taskId);
		Task AddNewTaskAsync(TasksUpdatingModel taskModel);
		Task UpdateTaskAsync(int taskId, TasksUpdatingModel taskModel);
		Task DeleteTaskAsync(int taskId);
	}
}
