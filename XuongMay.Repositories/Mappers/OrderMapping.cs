﻿using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
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
					   opt => opt.MapFrom(src => src.Product.Name)) // Lấy giá trị Name từ Product
				.ForMember(dest => dest.CreatedTime,
				opt => opt.MapFrom(src => src.CreatedTime.ToString("HH:mm dd/MM/yyyy")))  // Format CreatedTime
				.ForMember(dest => dest.StartTime,
				opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm dd/MM/yyyy")))
				.ForMember(dest => dest.EndTime,
				opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm dd/MM/yyyy")));

			//Ánh xạ từ AddOrderModelView sang Order
			CreateMap<AddOrderModelView, Order>()
				.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
				.ForMember(dest => dest.StartTime, opt => opt.Ignore())
				.ForMember(dest => dest.EndTime, opt => opt.Ignore())
				.ForMember(dest => dest.DeletedTime, opt => opt.Ignore());

			//Ánh xạ từ UpdateOrderModelView sang Order
			CreateMap<UpdateOrderModelView, Order>()
			.ForMember(dest => dest.StartTime, opt => opt.Ignore())
			.ForMember(dest => dest.EndTime, opt => opt.Ignore())
			.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedTime, opt => opt.Ignore());
		}
	}
}
