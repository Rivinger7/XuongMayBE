using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class ProductMapping : Profile
	{
		public ProductMapping()
		{

			CreateMap<Product, ResponseProductModel>()
			.ForMember(dest => dest.Category,
				opt => opt.MapFrom(src => src.Category.Name))  // Mapping Category Name
			.ForMember(dest => dest.CreatedTime,
				opt => opt.MapFrom(src => src.CreatedTime.ToString("HH:mm dd/MM/yyyy")))  // Format CreatedTime
			.ForMember(dest => dest.LastUpdateTime,
				opt => opt.MapFrom(src => src.LastUpdateTime.HasValue
					? src.LastUpdateTime.Value.ToString("HH:mm dd/MM/yyyy")
					: string.Empty))  // Format LastUpdateTime
			.ForMember(dest => dest.DeletedTime,
				opt => opt.MapFrom(src => src.DeletedTime.HasValue
					? src.DeletedTime.Value.ToString("HH:mm dd/MM/yyyy")
					: string.Empty));  // Format DeletedTime
			CreateMap<CreateProductModel, Product>()
			.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastUpdateTime, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
		}
	}
}
