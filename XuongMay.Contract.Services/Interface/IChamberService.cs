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

        Task CancelImportAsync(int inventoryHistoryId);

        Task CancelExportAsync(int inventoryHistoryId);

        Task TransferProduct(int productId, int itemsPerBox, int chamberId_1, int chamberId_2);

        Task UpdateProduct(int chamberId, int productId, int productIdNew, int itemPerBox);

        Task<BasePaginatedList<ChamberProductResponseModel>> GetProductsInChamberAsync(int pageNumber, int pageSize, int chamberID);
    }
}
