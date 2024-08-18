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
		List<AllCategoryModelView> GetAllCategory(bool? sortByName);

		AllCategoryModelView GetCategoryById(int id);

		AllCategoryModelView Add(AddCategoryModelView model);

		void Update(int id, AddCategoryModelView model);

		void Delete(int id);
	}
}
