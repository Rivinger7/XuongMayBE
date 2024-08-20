using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.TasksModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface ITasksService
	{
		Task<BasePaginatedList<Tasks>> GetAllTaskAsync(int pageIndex, int pageSize, bool? isCompleted, int? orderId);
		Task<TasksModel> GetTaskByIdAsync(int taskId);
		Task AddNewTaskAsync(TasksGeneralModel taskModel);
		Task UpdateTaskAsync(int taskId, TasksGeneralModel taskModel);
		Task DeleteTaskAsync(int taskId);
	}
}
