using AutoMapper;
using AutoMapper.Configuration.Conventions;
using GarmentFactory.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class ProductMapping : Profile
	{
		public ProductMapping()
		{

			CreateMap<Product, ResponseProductModel>()
				.ForMember(dest => dest.Category,
					   opt => opt.MapFrom(src => src.Category.Name)); // Lấy giá trị Name từ Category
			CreateMap<CreateProductModel, Product>()
			.ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
			.ForMember(dest => dest.LastUpdateTime, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedTime, opt => opt.Ignore())
			.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
		}
	}
}
