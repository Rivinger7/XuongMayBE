using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.WarehouseModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMay.Services.Service
{
	public class WarehouseService : IWarehouseService
	{
		private readonly IUnitOfWork _unitOfWork;
		public WarehouseService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


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
								   where chamberMapper.ChamberId == exportModel.ChamberId && !chamberMapper.DeletedTime.HasValue
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

			if(groupListChecking == null)
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
				CreatedBy = "Tester",
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
				CreatedBy = "Tester",
				ChamberProducts = chamberProduct,
				InventoryHistories = inventoryHistories
			};

			//thêm mới 2 cái vừa tạo và lưu 
			await _unitOfWork.GetRepository<InventoryChamberMappers>().InsertAsync(inventoryChamberMapper);
			await _unitOfWork.SaveAsync();


		}
	}
}
