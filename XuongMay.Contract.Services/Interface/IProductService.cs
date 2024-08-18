
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IProductService
	{
		Task<IEnumerable<ResponseProductModel>> GetAsync();
		Task<IEnumerable<ResponseProductModel>> GetProductsAsync(bool? sortByName);
		Task<ResponseProductModel> CreateProductAsync(CreateProductModel model);
		Task<ResponseProductModel> UpdateProductAsync(int id, CreateProductModel model);
		Task DeleteProductAsync(int id);
	}
}
