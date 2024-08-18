using AutoMapper;
using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.CategoryModels;

namespace XuongMay.Repositories.Mappers
{
	public class CategoryMapping : Profile
	{
		public CategoryMapping() 
		{
			CreateMap<Category, AllCategoryModelView>()
				.ForMember(dest => dest.Description, opt => opt.AllowNull());

			CreateMap<AddCategoryModelView, Category>()
				.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
		}
	}
}
