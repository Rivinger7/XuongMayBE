using XuongMay.Core;
using XuongMay.ModelViews.InventoryHistoriesModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IChamberService
	{
		Task<BasePaginatedList<ResponseInventoryHistoryModel>> GetInventoryHistoriesAsync(int pageNumber, int pageSize, int chamberID, int? searchId, string? searchProductName, bool? importAndExport);
	}
}
