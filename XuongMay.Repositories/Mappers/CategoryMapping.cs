using AutoMapper;
using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.CategoryModels;

namespace Dental_Clinic_System.Helper
{
	public class CategoryMapping : Profile
	{
		public CategoryMapping() 
		{
			CreateMap<Category, AllCategoryModel>()
				.ForMember(dest => dest.Description, opt => opt.AllowNull())
				.ForMember(dest => dest.CreatedTime,
				opt => opt.MapFrom(src => src.CreatedTime.ToString("HH:mm dd/MM/yyyy")));  // Format CreatedTime

			CreateMap<AddCategoryModel, Category>()
				.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
				.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
		}
	}
}
