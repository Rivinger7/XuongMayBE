
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.ChamberModelViews;
﻿using XuongMay.Core;
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
	}
}
