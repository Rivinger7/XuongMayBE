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
	}
}
