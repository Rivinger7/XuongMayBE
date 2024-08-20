using XuongMay.Core;
using XuongMay.ModelViews.OrderModelViews;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IOrderService
	{
		Task<BasePaginatedList<AllOrderModelView>> GetAllOrderAsync(int pageNumber, int pageSize, bool? isCompleted, string? productName);

		Task<AllOrderModelView> AddOrderAsync(AddOrderModelView model);

		Task UpdateOrderAsync (int id, UpdateOrderModelView model);

		Task DeleteOrderAsync (int id);
	}
}
