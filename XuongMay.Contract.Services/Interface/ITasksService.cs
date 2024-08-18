using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.TasksModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface ITasksService
	{
		Task<IList<Tasks>> GetAllTaskAsync();
		Task<Tasks> GetTaskByIdAsync(int taskId);
		Task AddNewTaskAsync(TasksCreateModel taskModel);
		Task DeleteTaskAsync(int taskId);
	}
}
