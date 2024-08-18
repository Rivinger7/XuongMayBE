using AutoMapper;
using AutoMapper.QueryableExtensions;
using GarmentFactory.Repository.Entities;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
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

		public List<AllOrderModelView> GetAllOrder(string searchByProductName)
		{
			// Tạo câu truy vấn IQueryable để lấy dữ liệu từ bảng Order trong database
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

			// Trả về danh sách các đơn hàng dưới dạng AllOrderModelView
			return orders
				.ProjectTo<AllOrderModelView>(_mapper.ConfigurationProvider)
				.ToList();
		}

		public AllOrderModelView AddOrder(AddOrderModelView model)
		{
			//Check số lượng không được để trống và <= 0
			if(model.Quantity <= 0)
			{
				throw new Exception("Số lượng đơn hàng phải lớn hơn 0.");
			}

			//Check thời gian bắt đầu phải nhỏ hơn thời gian kết thúc
			if (model.StartTime >= model.EndTime)
			{
				throw new Exception("Vui lòng điền thời gian bắt đầu nhỏ hơn thời gian kết thúc.");
			}

			//Check sản phẩm đã tồn tại chưa
			Product? existingProduct = _unitOfWork.GetRepository<Product>()
				.Entities
				.FirstOrDefault(p => p.Id == model.ProductId && !p.DeletedTime.HasValue)
				?? throw new Exception("Sản phẩm không tồn tại.");

			//Tạo đơn hàng mới
			Order newOrder = _mapper.Map<Order>(model);
			newOrder.ProductId = model.ProductId;
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

			//Check thời gian bắt đầu phải nhỏ hơn thời gian kết thúc
			if(model.StartTime >= model.EndTime)
			{
				throw new Exception("Vui lòng điền thời gian bắt đầu nhỏ hơn thời gian kết thúc.");
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

			//Cập nhật và lưu đơn hàng
			_mapper.Map(model, order);
			order.LastUpdatedTime = CoreHelper.SystemTimeNows;

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

			//Xóa mềm
			order.DeletedTime = CoreHelper.SystemTimeNows;

			_unitOfWork.GetRepository<Order>().Update(order);
			_unitOfWork.Save();
		}
	}
}
