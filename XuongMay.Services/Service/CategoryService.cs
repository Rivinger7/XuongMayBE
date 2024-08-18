using AutoMapper;
using AutoMapper.QueryableExtensions;
using GarmentFactory.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.CategoryModels;
using static XuongMay.Core.Base.BaseException;

namespace XuongMay.Services.Service
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public List<AllCategoryModel> GetAllCategory(bool? sortByName)
		{
			//Lấy tất cả các Category chưa bị xóa và sắp xếp theo CreaTime mới nhất
			IQueryable<Category> categories = _unitOfWork.GetRepository<Category>()
				.Entities
				.Where(c => !c.DeletedTime.HasValue)
				.OrderByDescending(c => c.CreatedTime);

			//Sắp xếp theo Name
			if (sortByName.HasValue)
			{
				categories = sortByName.Value
					? categories.OrderBy(c => c.Name)
					: categories.OrderByDescending(c => c.Name);
			}

			// Trả về list các Category đã sắp xếp dưới dạng AllCategoryModel
			return categories
				.ProjectTo<AllCategoryModel>(_mapper.ConfigurationProvider)
				.ToList();
		}

		public AllCategoryModel GetCategoryById(int id)
		{
			// Tìm Category theo Id, nếu không tìm thấy thì hiển thị thông báo
			Category category = _unitOfWork.GetRepository<Category>().GetById(id)
				?? throw new Exception("Danh mục không tồn tại");

			// Nếu Category bị xóa, hiển thị thông báo
			if (category.DeletedTime.HasValue)
			{
				throw new Exception("Không tìm thấy danh mục");
			}

			// Trả về thông tin category vừa được thêm dưới AllCategoryModelView
			return _mapper.Map<AllCategoryModel>(category);
		}

		public AllCategoryModel Add(AddCategoryModel model)
		{
			//Check tên không được để trống
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new Exception("Tên không được để trống!!!");
			}

			//Check category đã tồn tại hay chưa
			Category? existingCategory = _unitOfWork.GetRepository<Category>()
									 .Entities
									 .FirstOrDefault(c => c.Name == model.Name && !c.DeletedTime.HasValue);

			if (existingCategory != null)
			{
				throw new Exception($"Danh mục '{model.Name}' đã tồn tại.");
			}

			Category newCategory = _mapper.Map<Category>(model);
			newCategory.CreatedTime = CoreHelper.SystemTimeNows;
			newCategory.LastUpdatedTime = null;
			newCategory.DeletedTime = null;

			// Lưu category vào database
			_unitOfWork.GetRepository<Category>().Insert(newCategory);
			_unitOfWork.Save();

			// Trả về thông tin category vừa được thêm dưới AllCategoryModel
			return _mapper.Map<AllCategoryModel>(newCategory);
		}

		public void Update(int id, AddCategoryModel model)
		{
			//Check category có tồn tại không
			Category category = _unitOfWork.GetRepository<Category>().GetById(id)
				?? throw new Exception("Danh mục không tồn tại");

			//Check category có bị xóa chưa
			if (category.DeletedTime.HasValue)
			{
				throw new Exception("Không tìm thấy danh mục");
			}

			// Check Name không được để trống
			if (string.IsNullOrWhiteSpace(model.Name) )
			{
				throw new Exception("Tên không được để trống!!!");
			}

			//Check category có bị trùng tên không?
			Category? existingCategory = _unitOfWork.GetRepository<Category>()
									 .Entities
									 .FirstOrDefault(c => c.Name == model.Name && !c.DeletedTime.HasValue && c.Id != id);

			if (existingCategory != null)
			{
				throw new Exception($"Danh mục '{model.Name}' đã tồn tại.");
			}

			//Cập nhật và lưu danh mục
			_mapper.Map(model, category);
			category.LastUpdatedTime = CoreHelper.SystemTimeNows;

			_unitOfWork.GetRepository<Category>().Update(category);
			_unitOfWork.Save();
		}

		public void Delete(int id)
		{
			//Check category có tồn tại không
			Category category = _unitOfWork.GetRepository<Category>().GetById(id)
				?? throw new Exception("Danh mục không tồn tại");

			if(category.DeletedTime.HasValue)
			{
				throw new Exception("Không tìm thấy danh mục này");
			}

			//Check còn Product trong Category không? Nếu còn, thì ko thể xóa
			bool product = _unitOfWork.GetRepository<Product>()
				.Entities
				.Any(p => p.CategoryId == id && !p.DeletedTime.HasValue);
			
			if(product)
			{
				throw new Exception("Không thể xóa vì vẫn còn sản phẩm trong danh mục này");
			}

			//Xóa mềm
			category.DeletedTime = CoreHelper.SystemTimeNows;
			category.IsDeleted = true;

			_unitOfWork.GetRepository<Category>().Update(category);
			_unitOfWork.Save();
		}
	}
}
