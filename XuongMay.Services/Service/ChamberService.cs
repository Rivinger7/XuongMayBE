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
using System.Security.Claims;

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

		//Xuất kho
		public async Task ExportProductAsync(ExportProductModel exportModel)
		{

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
				CreatedBy = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value,
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

		//Hủy nhập kho
		public async Task CancelImportAsync(int inventoryHistoryId)
		{
			var inventoryHistories = await _unitOfWork.GetRepository<InventoryHistories>().GetByIdAsync(inventoryHistoryId)
										?? throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found history of product from Id you entered!" });

			// nếu là nhập kho thì isImport phải là true, ko phải false
			if(!inventoryHistories.IsImport)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest, new ErrorDetail() { ErrorMessage = "History of this Id is export!" });
			}

			// nếu đã bị xóa thì ko thể hủy dc
			if (inventoryHistories.DeletedTime.HasValue)
			{
				throw new ErrorException(StatusCodes.Status400BadRequest, new ErrorDetail() { ErrorMessage = "History of this Id is deleted!" });
			}

			//lấy mapper ra
			var inventoryChamberMapper = await _unitOfWork.GetRepository<InventoryChamberMappers>()
														  .Entities
														  .FirstOrDefaultAsync(i => i.InventoryId == inventoryHistoryId && !i.DeletedTime.HasValue)
										?? throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found any chamber mapper with given Inventory History Id" });

			//lấy chamber product tương ứng từ mapper
			var chamberProduct = await _unitOfWork.GetRepository<ChamberProducts>()
												  .Entities
												  .FirstOrDefaultAsync(c => c.Id == inventoryChamberMapper.ChamberId && !c.DeletedTime.HasValue)
										?? throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found any chamber base on inventory history Id" });

			//lấy tổng số lượng của import và export ra
			int sumQuantityImport = await _unitOfWork.GetRepository<InventoryChamberMappers>()
													.Entities
													.Include(i => i.InventoryHistories)
													.Include(i => i.ChamberProducts)
													.Where(i =>     i.InventoryId != inventoryHistoryId
																&&	i.InventoryHistories.IsImport == true
																&& !i.InventoryHistories.DeletedTime.HasValue	
																&& !i.ChamberProducts.DeletedTime.HasValue
																&& !i.DeletedTime.HasValue)
													.SumAsync(i => i.Quantity);

			Console.WriteLine(sumQuantityImport);

			int sumQuantityExport = await _unitOfWork.GetRepository<InventoryChamberMappers>()
													.Entities
													.Include(i => i.InventoryHistories)
													.Include(i => i.ChamberProducts)
													.Where(i => i.InventoryHistories.IsImport == false
																&& !i.InventoryHistories.DeletedTime.HasValue
																&& !i.ChamberProducts.DeletedTime.HasValue
																&& !i.DeletedTime.HasValue)
													.SumAsync(i => i.Quantity);
			Console.WriteLine(sumQuantityExport);

			//nếu tổng import của những lịch sử khác trừ đi tổng export mà lớn hơn hoặc = 0 thì OK, cho hủy
			if (sumQuantityImport - sumQuantityExport >= 0)
			{
				//cập nhật lại số lượng thùng hàng trong chamber
				chamberProduct.Quantity -= inventoryChamberMapper.Quantity;

				//cập nhật lại trạng thái đã xóa của lịch sử
				inventoryHistories.DeletedTime = CoreHelper.SystemTimeNows;
				inventoryHistories.DeletedBy = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

				//cập nhật lại trạng thái đã xóa của mappper
				inventoryChamberMapper.DeletedTime = CoreHelper.SystemTimeNows;
				inventoryChamberMapper.DeletedBy = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

				//lưu lại
				await _unitOfWork.SaveAsync();
			}
			else
			{
				throw new ErrorException(StatusCodes.Status400BadRequest, new ErrorDetail() { ErrorMessage = "Cannot cancel import because quantity of product in chamber is less than quantity of product in chamber" });
			}

		}

		//Hủy xuất kho
		public async Task CancelExportAsync(int inventoryHistoryId)
		{
			var inventoryHistories = await _unitOfWork.GetRepository<InventoryHistories>().GetByIdAsync(inventoryHistoryId)
										?? throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found inventory history !"});

			// nếu là nhập kho thì isImport phải là false dc
			if (inventoryHistories.IsImport) 
			{
				throw new ErrorException(StatusCodes.Status400BadRequest, new ErrorDetail() { ErrorMessage = "History of this Id is import!" });
			}

			// nếu đã bị xóa thì ko thể hủy dc
			if (inventoryHistories.DeletedTime.HasValue) 
			{
				throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found inventory history!!" });
			}

			//lấy mapper ra
			var inventoryChamberMapper = await _unitOfWork.GetRepository<InventoryChamberMappers>()
														  .Entities
														  .FirstOrDefaultAsync(i => i.InventoryId == inventoryHistoryId && !i.DeletedTime.HasValue)
										?? throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found any chamber mapper with given Inventory History Id" });

			//lấy chamber product tương ứng từ mapper
			var chamberProduct = await _unitOfWork.GetRepository<ChamberProducts>()
												  .Entities
												  .FirstOrDefaultAsync(c => c.Id == inventoryChamberMapper.ChamberId && !c.DeletedTime.HasValue)
										?? throw new ErrorException(StatusCodes.Status404NotFound, new ErrorDetail() { ErrorMessage = "Cannot found any chamber base on inventory history Id" });

			//cập nhật lại số lượng thùng hàng trong chamber
			chamberProduct.Quantity += inventoryChamberMapper.Quantity;

			//cập nhật lại trạng thái đã xóa của lịch sử
			inventoryHistories.DeletedTime = CoreHelper.SystemTimeNows;
			inventoryHistories.DeletedBy = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

			//cập nhật lại trạng thái đã xóa của mappper
			inventoryChamberMapper.DeletedTime = CoreHelper.SystemTimeNows;
			inventoryChamberMapper.DeletedBy = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

			//lưu lại
			await _unitOfWork.SaveAsync();

		}

		
	}
}

