﻿
using XuongMay.Core;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IProductService
	{
		Task<ResponseProductModel> GetProductAsync(int id);
		Task<BasePaginatedList<ResponseProductModel>> GetProductsAsync(int pageNumber, int pageSize, bool? sortByName);
		Task<BasePaginatedList<ResponseProductModel>> SearchProductsAsync(int pageNumber, int pageSize, string? name, string? category);
		Task<ResponseProductModel> CreateProductAsync(CreateProductModel model);
		Task<ResponseProductModel> UpdateProductAsync(int id, CreateProductModel model);
		Task<ResponseProductModel> DeleteProductAsync(int id);
	}
}
