using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.OrderModelViews;


namespace XuongMay.Services.Service
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<BasePaginatedList<AllOrderModelView>> GetAllOrderAsync(int pageNumber, int pageSize, bool? isCompleted, string? productName)
		{
			
			IQueryable<Order> ordersQuery = _unitOfWork.GetRepository<Order>().Entities
										.Where(o => !o.DeletedTime.HasValue);

			// Các order đã hoàn thành hay chưa nếu isCompleted có giá trị
			if (isCompleted.HasValue)
			{
				DateTime now = CoreHelper.SystemTimeNows;
				if (isCompleted.Value) 
				{
					//nếu isCompleted = true -> các order đã hoàn thành
					// Tổng quantity các task của order == quantity của order VÀ Endtime của mọi task của order <= now
					ordersQuery = ordersQuery.Where(o => _unitOfWork.GetRepository<Tasks>().Entities
																	.Where(t => !t.DeletedTime.HasValue && t.OrderId == o.Id)
																	.Sum(t => t.Quantity) == o.Quantity &&
														!_unitOfWork.GetRepository<Tasks>().Entities
																	.Where(t => !t.DeletedTime.HasValue && t.OrderId == o.Id)
																	.Any(t => t.EndTime > now));
				}
				else 
				{
					//nếu isCompleted = false -> các order chưa hoàn thành
					// Tổng quantity các task của order < quantity của order HOẶC có bất kỳ Endtime của mọi task của order > now
					ordersQuery = ordersQuery.Where(o => _unitOfWork.GetRepository<Tasks>().Entities
																	.Where(t => !t.DeletedTime.HasValue && t.OrderId == o.Id)
																	.Sum(t => t.Quantity) < o.Quantity ||
														_unitOfWork.GetRepository<Tasks>().Entities
																	.Where(t => !t.DeletedTime.HasValue && t.OrderId == o.Id)
					.Any(t => t.EndTime > now));
				}
			}

			//Lấy tất cả Order chưa bị xóa sắp xếp theo CreaTime mới nhất
			List<Order> orders = await ordersQuery.OrderByDescending(o => o.CreatedTime).ToListAsync();

			// Tìm kiếm theo Tên Sản Phẩm nếu productName có giá trị
			if (!string.IsNullOrWhiteSpace(productName))
			{
				productName = CoreHelper.ConvertVnString(productName);
				orders = orders.Where(o => CoreHelper.ConvertVnString(o.Product.Name).Contains(productName)).ToList();
			}

			// Đếm tổng số lượng đơn hàng sau khi đã lọc
			int totalOrders = orders.Count();

			// Áp dụng phân trang
			List<Order> pagedOrders = orders
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize).ToList();

			// Tạo BasePaginatedList và trả về
			IReadOnlyCollection<AllOrderModelView> responseItems = _mapper.Map<IReadOnlyCollection<AllOrderModelView>>(pagedOrders);
			return new BasePaginatedList<AllOrderModelView>(responseItems, totalOrders, pageNumber, pageSize);
		}

		public async Task<AllOrderModelView> AddOrderAsync(AddOrderModelView model)
		{
			//Check số lượng không được để trống và <= 0
			if (model.Quantity <= 0)
			{
				throw new Exception("Order quantity must be greater than 0.");
			}

			//Check sản phẩm đã tồn tại chưa
			Product? existingProduct = await _unitOfWork.GetRepository<Product>()
				.Entities
				.FirstOrDefaultAsync(p => p.Id == model.ProductId && !p.DeletedTime.HasValue)
				?? throw new Exception("Product does not exist.");

			//Check StartTime & EndTime không được để trống 
			if (string.IsNullOrWhiteSpace(model.StartTime) || string.IsNullOrWhiteSpace(model.EndTime))
			{
				throw new Exception("Start times and End times cannot be empty.");
			}
			DateTime startTime = TimeHelper.ConvertStringToDateTime(model.StartTime) ?? throw new Exception("Time entered in incorrect format HH:mm dd/MM/yyyy.");
			DateTime endTime = TimeHelper.ConvertStringToDateTime(model.EndTime) ?? throw new Exception("Time entered in incorrect format HH:mm dd/MM/yyyy.");

			//Check thời gian bắt đầu < thời gian kết thúc
			if (startTime >= endTime)
			{
				throw new Exception("Please enter start time less than end time.");
			}

			//Check thời gian bắt đầu > thời gian hiện tại
			if (startTime <= CoreHelper.SystemTimeNows)
			{
				throw new Exception("Start time must be greater than current time.");
			}

			//Tạo đơn hàng mới
			Order newOrder = _mapper.Map<Order>(model);
			newOrder.ProductId = model.ProductId;
			newOrder.StartTime = startTime;
			newOrder.EndTime = endTime;
			newOrder.CreatedTime = CoreHelper.SystemTimeNows;
			newOrder.LastUpdatedTime = null;
			newOrder.DeletedTime = null;

			//Lưu order vào DB
			await _unitOfWork.GetRepository<Order>().InsertAsync(newOrder);
			await _unitOfWork.SaveAsync();

			// Trả về thông tin đơn hàng vừa được thêm dưới dạng AllOrderModelView
			return _mapper.Map<AllOrderModelView>(newOrder);
		}

		public async Task UpdateOrderAsync(int id, UpdateOrderModelView model)
		{
			//Check số lượng không được để trống và <= 0
			if (model.Quantity <= 0)
			{
				throw new Exception("Order quantity must be greater than 0");
			}

			//Check đơn hàng đó có tồn tại không
			Order order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id)
			?? throw new Exception("Order not found");

			//Check đơn hàng đó đã bị xóa chưa
			if (order.DeletedTime.HasValue)
			{
				throw new Exception("Order not found");
			}

			//Check sản phẩm đó có tồn tại không
			Product product = await _unitOfWork.GetRepository<Product>()
				.Entities
				.FirstOrDefaultAsync(p => p.Id == model.ProductId && !p.DeletedTime.HasValue)
				?? throw new Exception("Product does not exist");

			//Check nếu Order đã có Task, không cho phép chỉnh sửa sản phẩm
			bool hasTasks = await _unitOfWork.GetRepository<Tasks>()
				.Entities
				.AnyAsync(t => t.OrderId == order.Id && !t.DeletedTime.HasValue);

			if (hasTasks && order.ProductId != model.ProductId)
			{
				throw new Exception("The product cannot be changed because the order has been assigned.");
			}

			//Tính tổng số lượng trong các Task của Order
			int totalQuantity = await _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Where(t => t.OrderId == order.Id && !t.DeletedTime.HasValue)
				.SumAsync(t => t.Quantity);

			// Check nếu Quantity của Order > tổng Quantity của các Task, thì được chỉnh sửa Quantity của Order
			if (model.Quantity < totalQuantity)
			{
				throw new Exception("The order quantity must be greater than or equal to the total quantity in the tasks.");
			}

			//Check StartTime & EndTime không được để trống 
			if (string.IsNullOrWhiteSpace(model.StartTime) || string.IsNullOrWhiteSpace(model.EndTime))
			{
				throw new Exception("Start times and End times cannot be empty.");
			}
			DateTime startTime = TimeHelper.ConvertStringToDateTime(model.StartTime) ?? throw new Exception("Time entered in incorrect format HH:mm dd/MM/yyyy.");
			DateTime endTime = TimeHelper.ConvertStringToDateTime(model.EndTime) ?? throw new Exception("Time entered in incorrect format HH:mm dd/MM/yyyy.");

			//Check thời gian bắt đầu < thời gian kết thúc
			if (startTime >= endTime)
			{
				throw new Exception("Please enter start time less than end time.");
			}

			//Check StartTime >= CreateTime
			if (startTime < order.CreatedTime)
			{
				throw new Exception("Start time must be greater than order creation time.");
			}

			//Lấy Task đầu tiên theo StartTime (Task có thời gian bắt đầu sớm nhất)
			Tasks? firstTask = await _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Where(t => t.OrderId == order.Id && !t.DeletedTime.HasValue)
				.OrderBy(t => t.StartTime)
				.FirstOrDefaultAsync();

			//Lấy Task cuối cùng theo EndTime (Task có thời gian kết thúc muộn nhất)
			Tasks? lastTask = await _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Where(t => t.OrderId == order.Id && !t.DeletedTime.HasValue)
				.OrderByDescending(t => t.EndTime)
				.FirstOrDefaultAsync();

			//Check StartTime của Order <= StartTime của Task đầu tiên
			if (firstTask != null && startTime > firstTask.StartTime)
			{
				throw new Exception("The Start time of the order must be less than or equal to the start time of the first task.");
			}

			// Check EndTime của Order > EndTime của Task cuối cùng
			if (lastTask != null && endTime <= lastTask.EndTime)
			{
				throw new Exception("The End time of the order must be greater than to the end time of the last task.");
			}

			//Cập nhật và lưu đơn hàng
			_mapper.Map(model, order);
			order.LastUpdatedTime = CoreHelper.SystemTimeNows;
			order.StartTime = startTime;
			order.EndTime = endTime;

			_unitOfWork.GetRepository<Order>().Update(order);
			await _unitOfWork.SaveAsync();

		}

		public async Task DeleteOrderAsync(int id)
		{
			//Check đơn hàng có tồn tại không
			Order order = await _unitOfWork.GetRepository<Order>().GetByIdAsync(id)
				?? throw new Exception("Order does not exist");

			//Check đơn hàng có bị xóa chưa
			if (order.DeletedTime.HasValue)
			{
				throw new Exception("Order does not exist");
			}

			//Check còn Task trong đơn hàng hay không? Nếu còn, ko thể xóa
			bool task = await _unitOfWork.GetRepository<Tasks>()
				.Entities
				.AnyAsync(t => t.OrderId == order.Id && !t.DeletedTime.HasValue);

			if (task)
			{
				throw new Exception("Cannot delete because there are still tasks on this order");
			}

			//Xóa mềm
			order.DeletedTime = CoreHelper.SystemTimeNows;

			_unitOfWork.GetRepository<Order>().Update(order);
			await _unitOfWork.SaveAsync();
		}
	}
}
