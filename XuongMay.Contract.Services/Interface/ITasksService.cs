using GarmentFactory.Repository.Entities;
using XuongMay.Core;
using XuongMay.ModelViews.TasksModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface ITasksService
	{
		Task<BasePaginatedList<Tasks>> GetAllTaskAsync(int pageIndex, int pageSize);
		Task<TasksModel> GetTaskByIdAsync(int taskId);
		Task AddNewTaskAsync(TasksCreateModel taskModel);
		Task DeleteTaskAsync(int taskId);
	}
}
