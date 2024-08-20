using XuongMay.Core;
using XuongMay.ModelViews.OrderModelViews;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IOrderService
	{
		Task<BasePaginatedList<AllOrderModelView>> GetAllOrder(int pageNumber, int pageSize, bool? isCompleted, string? productName);

		AllOrderModelView AddOrder(AddOrderModelView model);

		void UpdateOrder (int id, UpdateOrderModelView model);

		void DeleteOrder (int id);
	}
}
