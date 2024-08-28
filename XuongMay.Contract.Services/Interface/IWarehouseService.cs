using XuongMay.ModelViews.WarehouseModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IWarehouseService
	{
		Task ExportProductAsync(ExportProductModel exportModel);
	}
}
