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
using static XuongMay.Core.Base.BaseException;
using XuongMay.ModelViews.WarehouseModelViews;

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
			foreach (int productId in model.ProductIds)
			{
				Product product = await _unitOfWork.GetRepository<Product>().Entities.FirstOrDefaultAsync(p => p.Id == productId && !p.DeletedTime.HasValue)
									?? throw new Exception("The Product can not found!");
			}
			// Kiểm tra các chamberIds có tồn tại ko?
			foreach (int chamberId in model.ChamberIds)
			{
				ChamberProducts chamber = await _unitOfWork.GetRepository<ChamberProducts>().Entities.FirstOrDefaultAsync(c => c.Id == chamberId && !c.DeletedTime.HasValue)
											?? throw new Exception("The Chamber can not found!");
			}
			// Tạo đơn nhập kho cho từng product ứng vs từng chamber
			DateTime now = CoreHelper.SystemTimeNows;
			int userId = _contextAccessor.HttpContext.Session.GetInt32("userID") ?? throw new Exception("Login again!");
			User user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue);
			foreach (int productId in model.ProductIds)
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

				foreach (int chamberId in model.ChamberIds)
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

					//Cập nhật quantity trong ChamberProducts
					ChamberProducts? chamber = await _unitOfWork.GetRepository<ChamberProducts>().Entities.FirstOrDefaultAsync(c => c.Id == chamberId && !c.DeletedTime.HasValue);
					chamber.Quantity += model.Quantity;
					_unitOfWork.GetRepository<ChamberProducts>().Update(chamber);
					await _unitOfWork.SaveAsync();
				}
			}
		}

		#region Xem lịch sử xuất/nhập kho
		public async Task<BasePaginatedList<ResponseInventoryHistoryModel>> GetInventoryHistoriesAsync(int pageNumber, int pageSize, int chamberID, int? searchId, string? searchProductName, bool? importAndExport)
		{
			//Check chamber có tồn tại không
			ChamberProducts? chamber = await _unitOfWork.GetRepository<ChamberProducts>().GetByIdAsync(chamberID);

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
			else
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
		#endregion

		//Xuất kho
		public async Task ExportProductAsync(ExportProductModel exportModel)
		{

			// lấy ra user hiện tại
			int userId = _contextAccessor.HttpContext.Session.GetInt32("userID") ?? throw new ErrorException(StatusCodes.Status401Unauthorized,
										new ErrorDetail() { ErrorMessage = "You haven't loggin!" });
			User user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue);


			if (string.IsNullOrWhiteSpace(exportModel.InventoryName))
			{
				throw new ErrorException(StatusCodes.Status400BadRequest, new ErrorDetail() { ErrorMessage = "The inventory name must have at least 1 character" });
			}

			// lấy chamber ra theo Id
			var chamberProduct = await _unitOfWork.GetRepository<ChamberProducts>().GetByIdAsync(exportModel.ChamberId);

			if (chamberProduct == null)
			{
				throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot find any chamber with given Chamber Product Id" });
			}

			// join 2 bảng để select ra field cần thiết và quantity import - export
			var groupList = await (from chamberMapper in _unitOfWork.GetRepository<InventoryChamberMappers>().Entities
								   join inventoryHistory in _unitOfWork.GetRepository<InventoryHistories>().Entities
								   on chamberMapper.InventoryId equals inventoryHistory.Id
								   where chamberMapper.ChamberId == exportModel.ChamberId 
										&& !chamberMapper.DeletedTime.HasValue
										&& !inventoryHistory.DeletedTime.HasValue
								   group new { chamberMapper, inventoryHistory } by new
								   {
									   inventoryHistory.ProductId,
									   inventoryHistory.ItemPerBox
								   } into newGroup
								   select new
								   {
									   ProductId = newGroup.Key.ProductId,
									   ItemPerBox = newGroup.Key.ItemPerBox,
									   Quantity = newGroup.Where(x => x.inventoryHistory.IsImport == true).Sum(x => x.chamberMapper.Quantity)
												 - newGroup.Where(x => x.inventoryHistory.IsImport == false).Sum(x => x.chamberMapper.Quantity)
									   // lấy quantity import - quantity export
								   }).ToListAsync();


			var groupListChecking = groupList.Find(t => t.ProductId == exportModel.ProductId && t.ItemPerBox == exportModel.ItemPerBox);

			if (groupListChecking == null)
			{
				throw new ErrorException(StatusCodes.Status404NotFound,
										new ErrorDetail() { ErrorMessage = $"Cannot find any product in with given item per box {exportModel.ItemPerBox} in chamber" });
			}

			if (exportModel.Quantity > groupListChecking.Quantity || exportModel.Quantity <= 0)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest,
										new ErrorDetail() { ErrorMessage = "Quantity must greater than 0 and less than quantity in chamber" });
			}

			// cập nhật lại số lượng thùng hàng trong chamber
			chamberProduct.Quantity -= exportModel.Quantity;


			// thêm vào bảng lịch sử cái export
			var inventoryHistories = new InventoryHistories()
			{
				ProductId = exportModel.ProductId,
				TotalQuantity = exportModel.Quantity,
				ItemPerBox = exportModel.ItemPerBox,
				CreatedTime = CoreHelper.SystemTimeNows,
				CreatedBy = user.FullName,
				Name = exportModel.InventoryName,
				IsImport = false, // đây là export :>
			};
			await _unitOfWork.GetRepository<InventoryHistories>().InsertAsync(inventoryHistories);
			await _unitOfWork.SaveAsync(); //lưu trc, ko thì ko lấy được Id bỏ xuống dưới mapper

			// thêm mapper mới tương ứng
			var inventoryChamberMapper = new InventoryChamberMappers()
			{
				ChamberId = exportModel.ChamberId,
				InventoryId = inventoryHistories.Id,
				Quantity = exportModel.Quantity,
				CreatedTime = CoreHelper.SystemTimeNows,
				CreatedBy = user.FullName,
				ChamberProducts = chamberProduct,
				InventoryHistories = inventoryHistories
			};

			//thêm mới 2 cái vừa tạo và lưu 
			await _unitOfWork.GetRepository<InventoryChamberMappers>().InsertAsync(inventoryChamberMapper);
			await _unitOfWork.SaveAsync();


		}

		#region Cập nhật sản phẩm hiện tại sang 1 sản phẩm khác
		public async Task UpdateProduct(int chamberId, int productId, int productIdNew, int itemPerBox)
		{
			//Check khoang có tồn tại không
			ChamberProducts? chamber = _unitOfWork.GetRepository<ChamberProducts>().GetById(chamberId)
				?? throw new Exception("Khoang không tồn tại");

			//Check sản phẩm hiện tại có tồn tại không
			Product? product = _unitOfWork.GetRepository<Product>().GetById(productId)
				?? throw new Exception("Sản phẩm không tồn tại");

			//Check sản phẩm mới có tồn tại không
			Product? productNew = _unitOfWork.GetRepository<Product>().GetById(productIdNew) 
				?? throw new Exception("Sản phẩm mới không tồn tại");

			//Lấy danh sách InventoryChamberMapper của sản phẩm cũ
			List<InventoryChamberMappers> listMapper = await _unitOfWork.GetRepository<InventoryChamberMappers>()
				.Entities
				.Where(i => i.ChamberId == chamberId && !i.DeletedTime.HasValue 
							&& i.InventoryHistories.ProductId == productId && i.InventoryHistories.ItemPerBox == itemPerBox)
				.ToListAsync();

			//Check nếu không tìm thấy sản phẩm trong khoang để nâng cấp
			if (listMapper.Count == 0)
			{
				throw new Exception("Không tìm thấy sản phẩm trong khoang để nâng cấp");
			}

			int userId = _contextAccessor.HttpContext.Session.GetInt32("userID") ?? throw new Exception("Login again!");
			User? user = await _unitOfWork.GetRepository<User>().Entities.FirstOrDefaultAsync(u => u.Id == userId && !u.DeletedTime.HasValue);

			//Duyệt qua từng InventoryChamberMapper của sản phẩm cũ
			foreach (InventoryChamberMappers mapper in listMapper)
			{
				//Lấy thông tin InventoryHistories tương ứng với InventoryId của mapper
				InventoryHistories? history = _unitOfWork.GetRepository<InventoryHistories>().GetById(mapper.InventoryId);

				//Check nếu lịch sử nhập kho tồn tại và không bị xóa
				if (history.IsImport == true && !history.DeletedTime.HasValue)
				{
					//Xóa InventoryChamberMapper của sản phẩm cũ
					mapper.DeletedTime = CoreHelper.SystemTimeNows;
					_unitOfWork.GetRepository<InventoryChamberMappers>().Update(mapper);

					//Tạo mới lịch sử nhập kho cho sản phẩm mới
					InventoryHistories newInventoryHistory = new()
					{
						ProductId = productIdNew,
						Name = history.Name,
						Description = history.Description,
						IsImport = true,
						TotalQuantity = mapper.Quantity,
						CreatedBy = user.FullName,
						CreatedTime = CoreHelper.SystemTimeNows,
						ItemPerBox = itemPerBox
					};

					await _unitOfWork.GetRepository<InventoryHistories>().InsertAsync(newInventoryHistory);
					await _unitOfWork.SaveAsync();

					//Tạo InventoryChamberMapper cho sản phẩm mới
					InventoryChamberMappers invenChamberMapper = new() 
					{ 
						InventoryId = newInventoryHistory.Id,
						ChamberId = chamberId,
						Quantity = mapper.Quantity,
						CreatedBy = user.FullName,
						CreatedTime = CoreHelper.SystemTimeNows
					};
					await _unitOfWork.GetRepository<InventoryChamberMappers>().InsertAsync(invenChamberMapper);
					await _unitOfWork.SaveAsync();
				}
				else if (history.IsImport == false && !history.DeletedTime.HasValue)
				{
					history.ProductId = productIdNew;
					await _unitOfWork.GetRepository<InventoryHistories>().UpdateAsync(history);
					await _unitOfWork.SaveAsync();
				}
			}
		}
		#endregion
	}
}

