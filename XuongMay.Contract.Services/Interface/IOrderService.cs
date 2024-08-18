using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Core;
using XuongMay.ModelViews.OrderModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IOrderService
	{
		List<AllOrderModelView> GetAllOrder(string searchByProductName);

		AllOrderModelView AddOrder(AddOrderModelView model);

		void UpdateOrder (int id, UpdateOrderModelView model);

		void DeleteOrder (int id);
	}
}
