using AutoMapper;
using AutoMapper.QueryableExtensions;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.ChamberModelViews;
using XuongMay.ModelViews.InventoryHistoriesModelViews;

namespace XuongMay.Services.Service
{
	public class ChamberService : IChamberService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _contextAccessor;
		public ChamberService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_contextAccessor = contextAccessor;
		}
		// Nhập kho
		public async Task ImportChambers(ImportModel model)
		{
			// Kiểm tra Name đã tồn tại chưa
			IEnumerable<InventoryHistories> inventoryHistories = await _unitOfWork.GetRepository<InventoryHistories>().Entities.Where(i => !i.DeletedTime.HasValue).ToListAsync();
			if (inventoryHistories.Any(i => i.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)))
			{
				throw new Exception("Entered name already exists!");
			}
			// Kiểm tra đã nhập productID chưa
			if (model.ProductIds.Count == 0)
			{
				throw new Exception("Please select the product!");
			}
			// kiểm tra đã nhập khoang chưa
			if (model.ChamberIds.Count == 0)
			{
				throw new Exception("Please select the chamber!");
			}
			// Kiểm tra Quantity phải >= 0
			if (model.Quantity < 0)
			{
				throw new Exception("Quantity must be >= 0!");
			}
			// Kiểm tra ItemPerBox phải >= 0
			if (model.ItemPerBox < 0)
			{
				throw new Exception("Item Per Box must be >= 0!");
			}
			// Kiểm tra các productIds có tồn tại ko?
			foreach (var productId in model.ProductIds)
			{
				Product product = await _unitOfWork.GetRepository<Product>().Entities.FirstOrDefaultAsync(p => p.Id == productId && !p.DeletedTime.HasValue)
									?? throw new Exception("The Product can not found!");
			}
			// Kiểm tra các chamberIds có tồn tại ko?
			foreach (var chamberId in model.ChamberIds)
			{
				ChamberProducts chamber = await _unitOfWork.GetRepository<ChamberProducts>().Entities.FirstOrDefaultAsync(c => c.Id == chamberId && !c.DeletedTime.HasValue)
											?? throw new Exception("The Chamber can not found!");
			}
			// Tạo đơn nhập kho cho từng product ứng vs từng chamber
			DateTime now = CoreHelper.SystemTimeNows;
			int userId = _contextAccessor.HttpContext.Session.GetInt32("userID") ?? throw new Exception("Login again!");
			User user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue);
			foreach (var productId in model.ProductIds)
			{
				// Số dòng mới trong InventoryHistories = số productIds
				InventoryHistories import = new()
				{
					ProductId = productId,
					Name = model.Name,
					Description = model.Description,
					IsImport = true,
					TotalQuantity = model.Quantity * model.ChamberIds.Count,
					CreatedBy = user.FullName,
					CreatedTime = now,
					ItemPerBox = model.ItemPerBox
				};
				await _unitOfWork.GetRepository<InventoryHistories>().InsertAsync(import);
				await _unitOfWork.SaveAsync();

				// Lấy đơn nhập kho mới tạo
				import = await _unitOfWork.GetRepository<InventoryHistories>().Entities.FirstOrDefaultAsync(i => i.Name == model.Name && i.ProductId == productId && !i.DeletedTime.HasValue);

				foreach (var chamberId in model.ChamberIds)
				{
					// Số dòng mới trong InventoryChamberMappers = số ProductId * số ChamberIds
					InventoryChamberMappers newInventoryChamber = new()
					{
						InventoryId = import.Id,
						ChamberId = chamberId,
						Quantity = model.Quantity,
						CreatedBy = import.CreatedBy,
						CreatedTime = now
					};
					await _unitOfWork.GetRepository<InventoryChamberMappers>().InsertAsync(newInventoryChamber);
					await _unitOfWork.SaveAsync();

					//Cập nhật quantity trong ChamberProducts
					ChamberProducts? chamber = await _unitOfWork.GetRepository<ChamberProducts>().Entities.FirstOrDefaultAsync(c => c.Id == chamberId && !c.DeletedTime.HasValue);
					chamber.Quantity = chamber.Quantity + model.Quantity;
					_unitOfWork.GetRepository<ChamberProducts>().Update(chamber);
					await _unitOfWork.SaveAsync();
				}
			}
		}

		public async Task<BasePaginatedList<ResponseInventoryHistoryModel>> GetInventoryHistoriesAsync(int pageNumber, int pageSize, int chamberID, int? searchId, string? searchProductName, bool? importAndExport)
		{
			//Check chamber có tồn tại không
			ChamberProducts? chamber = await _unitOfWork.GetRepository<ChamberProducts>().GetByIdAsync(chamberID)
				?? throw new Exception("Khoang không tồn tại");

			//Tìm các InventoryChamberMapper có cùng chamberID
			IQueryable<InventoryChamberMappers> queryMapper = _unitOfWork.GetRepository<InventoryChamberMappers>()
				.Entities
				.Where(i => i.ChamberId == chamberID && !i.DeletedTime.HasValue);

			List<int> listMapper = await queryMapper.Select(i => i.InventoryId).ToListAsync();

			//Tạo truy vấn lịch sử nhập/xuất hàng và sắp xếp theo CreaTime mới nhất
			IQueryable<InventoryHistories> query = _unitOfWork.GetRepository<InventoryHistories>()
				.Entities
				.Where(i => listMapper.Contains(i.Id) && !i.DeletedTime.HasValue)
				.OrderByDescending(i => i.CreatedTime);

			// Lọc theo ImportAndExport: True là nhập kho. False là xuất kho
			if (importAndExport == true)
			{
				query = query.Where(i => i.IsImport == true);
			}
			else if (importAndExport == false)
			{
				query = query.Where(i => i.IsImport == false);
			}

			//lọc theo searchId
			if (searchId > 0)
			{
				query = query.Where(i => i.Id == searchId);
			}

			//Tìm kiếm theo ProductName nếu có giá trị
			if (!string.IsNullOrWhiteSpace(searchProductName))
			{
				query = query.Where(i => i.Product.Name.Contains(searchProductName));
			}

			//Check page Size không được < 0
			if (pageSize < 0)
			{
				throw new Exception("Kích thước trang phải lớn hơn 0");
			}

			// Đếm tổng số lượng sau khi đã lọc
			int totalCategory = await query.CountAsync();

			//Áp dụng phân trang
			IReadOnlyCollection<ResponseInventoryHistoryModel> pageInventoryHistory = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<ResponseInventoryHistoryModel>(_mapper.ConfigurationProvider)
				.ToListAsync();

			//Tạo BasePaginatedList và trả về
			return new BasePaginatedList<ResponseInventoryHistoryModel>(pageInventoryHistory, totalCategory, pageNumber, pageSize);
		}
	}
}

