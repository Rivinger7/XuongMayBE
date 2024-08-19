using AutoMapper;
using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.TasksModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class TasksMapping : Profile
	{
		public TasksMapping()
		{
			CreateMap<Tasks, TasksModel>().ReverseMap();

			CreateMap<Tasks, TasksGeneralModel>().ReverseMap();
		}
	}
}
