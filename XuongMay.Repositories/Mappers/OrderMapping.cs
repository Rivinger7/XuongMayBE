using AutoMapper;
using GarmentFactory.Repository.Entities;
using XuongMay.ModelViews.OrderModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class OrderMapping : Profile
	{
		public OrderMapping()
		{
			// Ánh xạ từ Order sang AllOrderModelView
			CreateMap<Order, AllOrderModelView>()
				.ForMember(dest => dest.ProductName,
					   opt => opt.MapFrom(src => src.Product.Name)); // Lấy giá trị Name từ Product

			//Ánh xạ từ AddOrderModelView sang Order
			CreateMap<AddOrderModelView, Order>()
				.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

			//Ánh xạ từ AddOrderModelView sang AllOrderModelView
			//CreateMap<AddOrderModelView, AllOrderModelView>()
			//	.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
			//	.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
			//	.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
			//	.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
			//	.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));

			//Ánh xạ từ UpdateOrderModelView sang Order
			CreateMap<UpdateOrderModelView, Order>()
			.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
			.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
			.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
			.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
		}
	}
}
