using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.Repositories.Mappers
{
	public class TasksMapping : Profile
	{
		public TasksMapping()
		{
			CreateMap<GarmentFactory.Repository.Entities.Tasks, XuongMay.ModelViews.TasksModelViews.TasksModel>().ReverseMap();
		}
	}
}
