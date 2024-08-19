using AutoMapper;
using AutoMapper.QueryableExtensions;
using GarmentFactory.Repository.Entities;
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

		public BasePaginatedList<AllOrderModelView> GetAllOrder(string searchByProductName, int pageNumber, int pageSize)
		{
			//Tạo câu truy vấn IQueryable để lấy dữ liệu từ bảng Order trong database
			//Lấy tất cả Order chưa bị xóa sắp xếp theo CreaTime mới nhất
			IQueryable<Order> orders = _unitOfWork.GetRepository<Order>()
				.Entities
				.Where(o => !o.DeletedTime.HasValue)
				.OrderByDescending(o => o.CreatedTime);

			//Tìm kiếm theo Tên Sản Phẩm nếu search có giá trị
			if (!string.IsNullOrWhiteSpace(searchByProductName))
			{
				orders = orders.Where(o => o.Product.Name.Contains(searchByProductName));
			}

			// Đếm tổng số lượng đơn hàng sau khi đã lọc
			int totalOrders = orders.Count();

			// Áp dụng phân trang
			List<AllOrderModelView> pagedOrders = orders
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<AllOrderModelView>(_mapper.ConfigurationProvider)
				.ToList();

			// Tạo BasePaginatedList và trả về
			return new BasePaginatedList<AllOrderModelView>(pagedOrders, totalOrders, pageNumber, pageSize);
		}

		public AllOrderModelView AddOrder(AddOrderModelView model)
		{
			//Check số lượng không được để trống và <= 0
			if(model.Quantity <= 0)
			{
				throw new Exception("Số lượng đơn hàng phải lớn hơn 0.");
			}

			//Check sản phẩm đã tồn tại chưa
			Product? existingProduct = _unitOfWork.GetRepository<Product>()
				.Entities
				.FirstOrDefault(p => p.Id == model.ProductId && !p.DeletedTime.HasValue)
				?? throw new Exception("Sản phẩm không tồn tại.");

			//Check StartTime & EndTime không được để trống 
			if (string.IsNullOrWhiteSpace(model.StartTime) || string.IsNullOrWhiteSpace(model.EndTime))
			{
				throw new Exception("Thời gian bắt đầu và kết thúc không được trống.");
			}
			DateTime startTime = TimeHelper.ConvertStringToDateTime(model.StartTime) ?? throw new Exception("Nhập thời gian không đúng định dạng HH:mm dd/MM/yyyy.");
			DateTime endTime = TimeHelper.ConvertStringToDateTime(model.EndTime) ?? throw new Exception("Nhập thời gian không đúng định dạng HH:mm dd/MM/yyyy.");

			//Check thời gian bắt đầu < thời gian kết thúc
			if (startTime >= endTime)
			{
				throw new Exception("Vui lòng điền thời gian bắt đầu nhỏ hơn thời gian kết thúc.");
			}

			//Check thời gian bắt đầu > thời gian hiện tại
			if (startTime <= CoreHelper.SystemTimeNows)
			{
				throw new Exception("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");
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
			_unitOfWork.GetRepository<Order>().Insert(newOrder);
			_unitOfWork.Save();

			// Trả về thông tin đơn hàng vừa được thêm dưới dạng AllOrderModelView
			return _mapper.Map<AllOrderModelView>(newOrder);
		}

		public void UpdateOrder(int id, UpdateOrderModelView model)
		{
			//Check số lượng không được để trống và <= 0
			if(model.Quantity <= 0)
			{
				throw new Exception("Số lượng đơn hàng phải lớn hơn 0");
			}

			//Check đơn hàng đó có tồn tại không
			Order order = _unitOfWork.GetRepository<Order>().GetById(id)
			?? throw new Exception("Không tìm thấy đơn hàng");

			//Check đơn hàng đó đã bị xóa chưa
			if (order.DeletedTime.HasValue)
			{
				throw new Exception("Không tìm thấy đơn hàng");
			}

			//Check sản phẩm đó có tồn tại không
			Product product = _unitOfWork.GetRepository<Product>()
				.Entities
				.FirstOrDefault(p => p.Id == model.ProductId && !p.DeletedTime.HasValue)
				?? throw new Exception("Sản phẩm không tồn tại");

			//Check nếu Order đã có Task, không cho phép chỉnh sửa sản phẩm
			bool hasTasks = _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Any(t => t.OrderId == order.Id && !t.DeletedTime.HasValue);

			if (hasTasks && order.ProductId != model.ProductId)
			{
				throw new Exception("Không thể thay đổi sản phẩm vì đơn hàng đã có nhiệm vụ được giao.");
			}

			//Tính tổng số lượng trong các Task của Order
			int totalQuantity = _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Where(t => t.OrderId == order.Id && !t.DeletedTime.HasValue)
				.Sum(t => t.Quantity);

			// Check nếu Quantity của Order > tổng Quantity của các Task, thì được chỉnh sửa Quantity của Order
			if(model.Quantity < totalQuantity)
			{
				throw new Exception("Số lượng đơn hàng phải lớn hơn hoặc bằng tổng số lượng trong các nhiệm vụ");
			}

			//Check StartTime & EndTime không được để trống 
			if (string.IsNullOrWhiteSpace(model.StartTime) || string.IsNullOrWhiteSpace(model.EndTime))
			{
				throw new Exception("Thời gian bắt đầu và kết thúc không được trống.");
			}
			DateTime startTime = TimeHelper.ConvertStringToDateTime(model.StartTime) ?? throw new Exception("Nhập thời gian không đúng định dạng HH:mm dd/MM/yyyy.");
			DateTime endTime = TimeHelper.ConvertStringToDateTime(model.EndTime) ?? throw new Exception("Nhập thời gian không đúng định dạng HH:mm dd/MM/yyyy.");

			//Check thời gian bắt đầu < thời gian kết thúc
			if (startTime >= endTime)
			{
				throw new Exception("Vui lòng điền thời gian bắt đầu nhỏ hơn thời gian kết thúc.");
			}

			//Check StartTime >= CreateTime
			if(startTime < order.CreatedTime)
			{
				throw new Exception("Thời gian bắt đầu phải lớn hơn thời gian tạo đơn hàng.");
			}

			//Lấy Task đầu tiên theo StartTime (Task có thời gian bắt đầu sớm nhất)
			Tasks? firstTask = _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Where(t => t.OrderId == order.Id && !t.DeletedTime.HasValue)
				.OrderBy(t => t.StartTime)
				.FirstOrDefault();

			//Lấy Task cuối cùng theo EndTime (Task có thời gian kết thúc muộn nhất)
			Tasks? lastTask = _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Where(t => t.OrderId == order.Id && !t.DeletedTime.HasValue)
				.OrderByDescending(t => t.EndTime)
				.FirstOrDefault();

			//Check StartTime của Order <= StartTime của Task đầu tiên
			if(firstTask != null && startTime > firstTask.StartTime)
			{
				throw new Exception("Thời gian bắt đầu của đơn hàng phải nhỏ hơn hoặc bằng thời gian bắt đầu của nhiệm vụ đầu tiên.");
			}

			// Check EndTime của Order > EndTime của Task cuối cùng
			if (lastTask != null && endTime <= lastTask.EndTime)
			{
				throw new Exception("Thời gian kết thúc của đơn hàng phải lớn hơn thời gian kết thúc của nhiệm vụ cuối cùng.");
			}

			//Cập nhật và lưu đơn hàng
			_mapper.Map(model, order);
			order.LastUpdatedTime = CoreHelper.SystemTimeNows;
			order.StartTime = startTime;
			order.EndTime = endTime;

			_unitOfWork.GetRepository<Order>().Update(order);
			_unitOfWork.Save();

		}

		public void DeleteOrder(int id)
		{
			//Check đơn hàng có tồn tại không
			Order order = _unitOfWork.GetRepository<Order>().GetById(id)
				?? throw new Exception("Đơn hàng không tồn tại");

			//Check đơn hàng có bị xóa chưa
			if(order.DeletedTime.HasValue)
			{
				throw new Exception("Đơn hàng không tồn tại");
			}

			//Check còn Task trong đơn hàng hay không? Nếu còn, ko thể xóa
			bool task = _unitOfWork.GetRepository<Tasks>()
				.Entities
				.Any(t => t.OrderId == order.Id && !t.DeletedTime.HasValue);

			if (task)
			{
				throw new Exception("Không thể xóa vì vẫn còn nhiệm vụ trong đơn hàng này");
			}

			//Xóa mềm
			order.DeletedTime = CoreHelper.SystemTimeNows;

			_unitOfWork.GetRepository<Order>().Update(order);
			_unitOfWork.Save();
		}
	}
}
