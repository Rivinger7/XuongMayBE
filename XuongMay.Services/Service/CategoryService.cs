using AutoMapper;
using AutoMapper.QueryableExtensions;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.CategoryModels;

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

		public async Task<BasePaginatedList<AllCategoryModel>> GetAllCategoryAsync(int? id, bool? sortByName, int pageNumber, int pageSize)
		{
			//Lấy tất cả các Category chưa bị xóa và sắp xếp theo CreaTime mới nhất
			IQueryable<Category> categories = _unitOfWork.GetRepository<Category>()
				.Entities
				.Where(c => !c.DeletedTime.HasValue)
				.OrderByDescending(c => c.CreatedTime);

			//Lọc theo ID nếu ID có giá trị
			if (id.HasValue)
			{
				categories = categories.Where(c => c.Id == id.Value);
			}

			//Sắp xếp theo Name
			if (sortByName.HasValue)
			{
				categories = sortByName.Value
					? categories.OrderBy(c => c.Name)
					: categories.OrderByDescending(c => c.Name);
			}

			// Đếm tổng số lượng đơn hàng sau khi đã lọc
			int totalCategory = await categories.CountAsync();

			//Áp dụng phân trang
			List<AllCategoryModel> pageCategory = await categories
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<AllCategoryModel>(_mapper.ConfigurationProvider)
				.ToListAsync(); ;

			// Tạo BasePaginatedList và trả về
			return new BasePaginatedList<AllCategoryModel>(pageCategory, totalCategory, pageNumber, pageSize);
		}

		public async Task<AllCategoryModel> AddAsync(AddCategoryModel model)
		{
			//Check tên không được để trống
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new Exception("Name cannot be blank!!!");
			}

			//Check category đã tồn tại hay chưa
			Category? existingCategory = await _unitOfWork.GetRepository<Category>()
									 .Entities
									 .FirstOrDefaultAsync(c => c.Name == model.Name && !c.DeletedTime.HasValue);

			if (existingCategory != null)
			{
				throw new Exception($"The category '{model.Name}' already exists.");
			}

			Category newCategory = _mapper.Map<Category>(model);
			newCategory.CreatedTime = CoreHelper.SystemTimeNows;
			newCategory.LastUpdatedTime = null;
			newCategory.DeletedTime = null;

			// Lưu category vào database
			await _unitOfWork.GetRepository<Category>().InsertAsync(newCategory);
			await _unitOfWork.SaveAsync();

			// Trả về thông tin category vừa được thêm dưới AllCategoryModel
			return _mapper.Map<AllCategoryModel>(newCategory);
		}

		public async Task UpdateAsync(int id, AddCategoryModel model)
		{
			//Check category có tồn tại không
			Category category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id)
				?? throw new Exception("Category does not exist");

			//Check category có bị xóa chưa
			if (category.DeletedTime.HasValue)
			{
				throw new Exception("No category found");
			}

			// Check Name không được để trống
			if (string.IsNullOrWhiteSpace(model.Name) )
			{
				throw new Exception("Name cannot be blank!!!");
			}

			//Check category có bị trùng tên không?
			Category? existingCategory = await _unitOfWork.GetRepository<Category>()
				.Entities
				.FirstOrDefaultAsync(c => c.Name == model.Name && !c.DeletedTime.HasValue && c.Id != id);

			if (existingCategory != null)
			{
				throw new Exception($"The category '{model.Name}' already exists.");
			}

			//Check có Product trong Category không? Nếu có, thì không thể update
			bool product = await _unitOfWork.GetRepository<Product>()
				.Entities
				.AnyAsync(p => p.CategoryId == id && !p.DeletedTime.HasValue);
				
			if(product)
			{
				throw new Exception("Cannot update because there are still products in this category");
			}

			//Cập nhật và lưu danh mục
			_mapper.Map(model, category);
			category.LastUpdatedTime = CoreHelper.SystemTimeNows;

			_unitOfWork.GetRepository<Category>().Update(category);
			await _unitOfWork.SaveAsync();
		}

		public async Task DeleteAsync(int id)
		{
			//Check category có tồn tại không
			Category category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id)
				?? throw new Exception("Category does not exist");

			if(category.DeletedTime.HasValue)
			{
				throw new Exception("This category was not found.");
			}

			//Check còn Product trong Category không? Nếu còn, thì ko thể xóa
			bool product = await _unitOfWork.GetRepository<Product>()
				.Entities
				.AnyAsync(p => p.CategoryId == id && !p.DeletedTime.HasValue);
			
			if(product)
			{
				throw new Exception("Cannot delete because there are still products in this category");
			}

			//Xóa mềm
			category.DeletedTime = CoreHelper.SystemTimeNows;
			category.IsDeleted = true;

			_unitOfWork.GetRepository<Category>().Update(category);
			await _unitOfWork.SaveAsync();
		}
	}
}
