using AutoMapper;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.ModelViews.InventoryHistoriesModelViews;

namespace XuongMay.Repositories.Mappers
{
	public class InventoryHistoriesMapping : Profile
	{
		public InventoryHistoriesMapping() 
		{
			// Ánh xạ từ InventoryHistories sang ResponseInventoryHistorie
			CreateMap<InventoryHistories, ResponseInventoryHistoryModel>()
				.ForMember(dest => dest.ProducName, opt => opt.MapFrom(src => src.Product.Name))
				.ForMember(dest => dest.CreatedTime,
				opt => opt.MapFrom(src => src.CreatedTime.ToString("HH:mm dd/MM/yyyy")))  // Format CreatedTime
				.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.InventoryChamberMappers.FirstOrDefault(m => m.InventoryId == src.Id).Quantity));
		}


	}
}
