using XuongMay.Core;
using XuongMay.ModelViews.OrderModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IOrderService
	{
		BasePaginatedList<AllOrderModelView> GetAllOrder(string searchByProductName, int pageNumber, int pageSize);

		AllOrderModelView AddOrder(AddOrderModelView model);

		void UpdateOrder (int id, UpdateOrderModelView model);

		void DeleteOrder (int id);
		Task<BasePaginatedList<AllOrderModelView>> GetCompletedOrder(int pageNumber, int pageSize);
		Task<BasePaginatedList<AllOrderModelView>> GetIncompletedOrder(int pageNumber, int pageSize);
		Task<BasePaginatedList<AllOrderModelView>> GetOrderByProductName(int pageNumber, int pageSize, string? productName);
	}
}
