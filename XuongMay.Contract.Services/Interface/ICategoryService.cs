using XuongMay.Core;
using XuongMay.ModelViews.CategoryModels;

namespace XuongMay.Contract.Services.Interface
{
	public interface ICategoryService
	{
		BasePaginatedList<AllCategoryModel> GetAllCategory(int? id, bool? sortByName, int pageNumber, int pageSize);

		AllCategoryModel Add(AddCategoryModel model);

		void Update(int id, AddCategoryModel model);

		void Delete(int id);
	}
}
