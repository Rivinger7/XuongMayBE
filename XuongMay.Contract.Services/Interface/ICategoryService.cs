using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.CategoryModels;

namespace XuongMay.Contract.Services.Interface
{
	public interface ICategoryService
	{
		List<AllCategoryModel> GetAllCategory(bool? sortByName);

		AllCategoryModel Add(AddCategoryModel model);

		void Update(int id, AddCategoryModel model);

		void Delete(int id);
	}
}
