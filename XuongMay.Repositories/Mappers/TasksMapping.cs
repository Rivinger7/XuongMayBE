using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.Core;
using XuongMay.ModelViews.TasksModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class TasksMapping : Profile
	{
		public TasksMapping()
		{
			//config for mapping between Tasks and TasksGettingModel
			CreateMap<Tasks, TasksGettingModel>()
				.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm dd/MM/yyyy"))) // change format from DateTime to string
				.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm dd/MM/yyyy")))
				.ForMember(dest => dest.LastUpdatedTime,
								   opt => opt.MapFrom(src => src.LastUpdatedTime.HasValue
															? src.LastUpdatedTime.Value.ToString("HH:mm dd/MM/yyyy")
															: string.Empty));
															//.ReverseMap();

			//config for mapping between Tasks and TasksUpdatingModel
			CreateMap<Tasks, TasksUpdatingModel>()
				.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm dd/MM/yyyy")))
				.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm dd/MM/yyyy")));


			//config for paginated list between Tasks and TasksGettingModel
			CreateMap<BasePaginatedList<Tasks>, BasePaginatedList<TasksGettingModel>>()
				.ConvertUsing((src, dest, context) =>
				{
					//map the list of Tasks to list of TasksGettingModel
					var mappedItems = context.Mapper.Map<List<TasksGettingModel>>(src.Items);
					return new BasePaginatedList<TasksGettingModel>(mappedItems, src.TotalItems, src.CurrentPage, src.PageSize);
				});
		}
	}
}
