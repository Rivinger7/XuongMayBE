using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class ProductMapping : Profile
	{
		public ProductMapping()
		{
			// Truyền các giá trị từ Product sang ResponseProductModel
			CreateMap<Product, ResponseProductModel>()
			.ForMember(dest => dest.Category,
				opt => opt.MapFrom(src => src.Category.Name))  // Lấy tên thể loại thay vì id
			.ForMember(dest => dest.CreatedTime,
				opt => opt.MapFrom(src => src.CreatedTime.ToString("HH:mm dd/MM/yyyy")))  // định dạng time vd: 14:00 21/08/2024
			.ForMember(dest => dest.LastUpdateTime,
				opt => opt.MapFrom(src => src.LastUpdateTime.HasValue
					? src.LastUpdateTime.Value.ToString("HH:mm dd/MM/yyyy") : string.Empty))  // định dạng time nếu có giá trị
			.ForMember(dest => dest.DeletedTime,
				opt => opt.MapFrom(src => src.DeletedTime.HasValue
					? src.DeletedTime.Value.ToString("HH:mm dd/MM/yyyy") : string.Empty));  // định dạng time nếu có giá trị

			// Truyền các giá trị từ CreateProductModel sang Product
			CreateMap<CreateProductModel, Product>()
			.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastUpdateTime, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
		}
	}
}
