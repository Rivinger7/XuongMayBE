using XuongMay.Core;
using XuongMay.ModelViews.CategoryModels;

namespace XuongMay.Contract.Services.Interface
{
	public interface ICategoryService
	{
		Task<BasePaginatedList<AllCategoryModel>> GetAllCategoryAsync(int? id, bool? sortByName, int pageNumber, int pageSize);

		Task<AllCategoryModel> AddAsync(AddCategoryModel model);

		Task UpdateAsync(int id, AddCategoryModel model);

		Task DeleteAsync(int id);
	}
}
