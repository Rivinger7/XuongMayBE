using XuongMay.Core;
using XuongMay.ModelViews.ChamberModelViews;
using XuongMay.ModelViews.InventoryHistoriesModelViews;
using XuongMay.ModelViews.WarehouseModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IChamberService
	{
		Task ImportChambers(ImportModel model);

		Task<BasePaginatedList<ResponseInventoryHistoryModel>> GetInventoryHistoriesAsync(int pageNumber, int pageSize, int chamberID, int? searchId, string? searchProductName, bool? importAndExport);

		Task ExportProductAsync(ExportProductModel exportModel);

		Task UpdateProduct(int chamberId, int productId, int productIdNew, int itemPerBox);
	}
}
