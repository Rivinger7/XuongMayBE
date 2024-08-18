
using XuongMay.Core;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IProductService
	{
		Task<BasePaginatedList<ResponseProductModel>> GetAsync(int pageNumber, int pageSize);
		Task<BasePaginatedList<ResponseProductModel>> GetProductsAsync(int pageNumber, int pageSize, bool? sortByName);
		//Task<IEnumerable<ResponseProductModel>> GetAsync();
		//Task<IEnumerable<ResponseProductModel>> GetProductsAsync(bool? sortByName);
		Task<ResponseProductModel> CreateProductAsync(CreateProductModel model);
		Task<ResponseProductModel> UpdateProductAsync(int id, CreateProductModel model);
		Task DeleteProductAsync(int id);
	}
}
