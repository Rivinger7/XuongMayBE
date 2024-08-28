using AutoMapper;
using GarmentFactory.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.Core.Utils;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Services.Service
{
	public class ProductService : IProductService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		public ProductService (IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		// Lấy sản phẩm theo id
		public async Task<ResponseProductModel> GetProductAsync(int id)
		{
			// Lấy sản phẩm - kiểm tra sự tồn tại
			Product product = await _unitOfWork.GetRepository<Product>().Entities.FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue) ?? throw new Exception("The Prodcut can not found!");
			return _mapper.Map<ResponseProductModel>(product);
		}

		// Lấy danh sách các sản phẩm chưa bị xóa, sort nếu muốn
		public async Task<BasePaginatedList<ResponseProductModel>> GetProductsAsync(int pageNumber, int pageSize, bool? sortByName)
		{
			IQueryable<Product> productsQuery = _unitOfWork.GetRepository<Product>().Entities.Include(p => p.Category).Where(p => !p.DeletedTime.HasValue).OrderByDescending(p => p.CreatedTime);
			// Sắp xếp theo Name 
			if (sortByName.HasValue)
			{
				//nếu sortByName = true -> xếp tăng dần
				//nếu sortByName = false -> xếp giảm dần
				productsQuery = sortByName.Value ? productsQuery.OrderBy(p => p.Name) : productsQuery.OrderByDescending(p => p.Name);
			}

			// Tổng số phần tử
			int totalCount = await productsQuery.CountAsync();
			// Áp dụng pagination
			List<Product> paginatedProducts = await productsQuery
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			IReadOnlyCollection<ResponseProductModel> responseItems = _mapper.Map<IReadOnlyCollection<ResponseProductModel>>(paginatedProducts);
			return new BasePaginatedList<ResponseProductModel>(responseItems, totalCount, pageNumber, pageSize);
		}

		// Tìm các sản phẩm theo tên và thể loại - nếu ko nhập tên hoặc thể loại nào, in ra tất cả
		public async Task<BasePaginatedList<ResponseProductModel>> SearchProductsAsync(int pageNumber, int pageSize, string? name, string? category)
		{
			List<Product> products = await _unitOfWork.GetRepository<Product>().Entities.Include(p => p.Category).Where(p => !p.DeletedTime.HasValue).ToListAsync();
			// Tìm theo tên sản phẩm 
			if (!string.IsNullOrWhiteSpace(name))
			{
				name = CoreHelper.ConvertVnString(name);
				products = products.Where(p => CoreHelper.ConvertVnString(p.Name).Contains(name)).ToList();
			}
			// Tìm theo tên thể loại
			if (!string.IsNullOrWhiteSpace(category))
			{
				category = CoreHelper.ConvertVnString(category);
				products = products.Where(p => CoreHelper.ConvertVnString(p.Category.Name).Contains(category)).ToList();
			}

			// Tổng số phần tử
			int totalCount = products.Count();

			// Áp dụng pagination
			List<Product> paginatedProducts = products
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize).ToList();

			IReadOnlyCollection<ResponseProductModel> responseItems = _mapper.Map<IReadOnlyCollection<ResponseProductModel>>(paginatedProducts);
			return new BasePaginatedList<ResponseProductModel>(responseItems, totalCount, pageNumber, pageSize);
		}

		// Tạo 1 product mới
		public async Task<ResponseProductModel> CreateProductAsync(CreateProductModel model)
		{
			// Kiểm tra tên không được để trống
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new Exception("Please enter product name!");
			}
			// Kiểm tra categoryId không được để trống
			if (string.IsNullOrWhiteSpace(model.CategoryId.ToString()))
			{
				throw new Exception("Please select a category!");
			}
            // Kiểm tra sản phẩm đã tồn tại hay chưa
			var products = await _unitOfWork.GetRepository<Product>().Entities.ToListAsync();
			bool isExistProduct = products.Any(p => p.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase) && !p.DeletedTime.HasValue);
			if (isExistProduct)
			{
				throw new Exception("Entered name already exists!");
			}
			// Kiểm tra categoryId phải tồn tại trong Category
			bool isExistCategory = await _unitOfWork.GetRepository<Category>().Entities.AnyAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue);
			if (!isExistCategory)
			{
				throw new Exception("The Category can not found!");
			}
			// Lưu sản phẩm vào DB
			Product newProduct = _mapper.Map<Product>(model);
			newProduct.CreatedTime = CoreHelper.SystemTimeNows;
			await _unitOfWork.GetRepository<Product>().InsertAsync(newProduct);
			await _unitOfWork.SaveAsync();

			Product? product = await _unitOfWork.GetRepository<Product>().Entities.Include(p => p.Category).FirstOrDefaultAsync(p => p.Name == model.Name);
			return _mapper.Map<ResponseProductModel>(product);
		}
		// Cập nhật 1 sản phẩm 
		public async Task<ResponseProductModel> UpdateProductAsync(int id, CreateProductModel model)
		{
			// Lấy sản phẩm - kiểm tra sự tồn tại
			Product product = await _unitOfWork.GetRepository<Product>().Entities.FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue) ?? throw new Exception("The Product can not found!");

			// Kiểm tra tên không được để trống
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new Exception("Please enter product name!");
			}
			// Kiểm tra categoryId không được để trống
			if (string.IsNullOrWhiteSpace(model.CategoryId.ToString()))
			{
				throw new Exception("Please select a category!");
			}
			// Kiểm tra tên mới của sản phẩm có bị trùng tên sản phẩm khác
			var products = await _unitOfWork.GetRepository<Product>().Entities.ToListAsync();
			bool isExistProductName = products.Any(p => p.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase) && !p.DeletedTime.HasValue && p.Id != id);
			if (isExistProductName)
			{
				throw new Exception("Entered name already exists!");
			}
			// Kiểm tra categoryId phải tồn tại trong Category
			bool isExistCategory = await _unitOfWork.GetRepository<Category>().Entities.AnyAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue);
			if (!isExistCategory)
			{
				throw new Exception("The Category can not found!");
			}
			//Cập nhật và lưu sản phẩm vào db
			_mapper.Map(model, product);
			product.LastUpdateTime = CoreHelper.SystemTimeNows;
			_unitOfWork.GetRepository<Product>().Update(product);
			await _unitOfWork.SaveAsync();

			return _mapper.Map<ResponseProductModel>(product);
		}

		// Xóa 1 sản phẩm
		public async Task<ResponseProductModel> DeleteProductAsync(int id)
		{
			// Lấy sản phẩm - kiểm tra sự tồn tại
			Product product = await _unitOfWork.GetRepository<Product>().Entities.FirstOrDefaultAsync(p => p.Id == id && !p.DeletedTime.HasValue) ?? throw new Exception("The Prodcut can not found!");

			//Xóa mềm
			product.DeletedTime = CoreHelper.SystemTimeNows;
			_unitOfWork.GetRepository<Product>().Update(product);
			await _unitOfWork.SaveAsync();

			return _mapper.Map<ResponseProductModel>(product);
		}
	} 
}
