using AutoMapper;
using GarmentFactory.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.Contract.Services.Interface;
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
		// Lấy danh sách mọi sản phẩm (cả những sản phẩm bị xóa)
		public List<ResponseProductModel> Get()
		{
			IEnumerable<Product> products = _unitOfWork.GetRepository<Product>().Entities.Include(p => p.Category);
			return _mapper.Map<List<ResponseProductModel>>(products.ToList());
		}

		// Lấy danh sách các sản phẩm chưa bị xóa, sort nếu muốn
		public List<ResponseProductModel> GetProducts(bool? sortByName)
		{
			IQueryable<Product> products = _unitOfWork.GetRepository<Product>().Entities.Include(p => p.Category).Where(p => !p.DeletedTime.HasValue).OrderByDescending(p => p.CreatedTime);
			// Sắp xếp theo Name 
			if (sortByName.HasValue)
			{
				//nếu sortByName = true -> xếp tăng dần
				//nếu sortByName = false -> xếp giảm dần
				products = sortByName.Value ? products.OrderBy(p => p.Name) : products.OrderByDescending(p => p.Name);
			}
			return _mapper.Map<List<ResponseProductModel>>(products.ToList());
		}

		// Tạo 1 product mới
		public ResponseProductModel CreateProduct(CreateProductModel model)
		{
			// Kiểm tra tên không được để trống
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new Exception("Vui lòng nhập tên sản phẩm.");
			}
			// Kiểm tra categoryId không được để trống
			if (string.IsNullOrWhiteSpace(model.CategoryId.ToString()))
			{
				throw new Exception("Vui lòng chọn thể loại.");
			}
			// Kiểm tra sản phẩm đã tồn tại hay chưa
			bool isExistProduct = _unitOfWork.GetRepository<Product>().Entities.Any(p => p.Name == model.Name && !p.DeletedTime.HasValue);
			if (isExistProduct)
			{
				throw new Exception("Sản phẩm có tên " + model.Name + " đã tồn tại.");
			}
			// Lưu sản phẩm vào DB
			var product = _mapper.Map<Product>(model);
			product.CreatedTime = CoreHelper.SystemTimeNows;
			product.IsDeleted = false;
			_unitOfWork.GetRepository<Product>().Insert(product);
			_unitOfWork.Save();

			product = _unitOfWork.GetRepository<Product>().Entities.Include(p => p.Category).FirstOrDefault(p => p.Name == model.Name);
			return _mapper.Map<ResponseProductModel>(product);
		}
		// Cập nhật 1 sản phẩm 
		public void UpdateProduct(int id, CreateProductModel model)
		{
			// Lấy sản phẩm - kiểm tra sự tồn tại
			Product product = _unitOfWork.GetRepository<Product>().Entities.FirstOrDefault(p => p.Id == id && !p.DeletedTime.HasValue) ?? throw new Exception("Không tìm thấy sản phẩm.");

			// Kiểm tra tên không được để trống
			if (string.IsNullOrWhiteSpace(model.Name))
			{
				throw new Exception("Vui lòng nhập tên sản phẩm.");
			}
			// Kiểm tra categoryId không được để trống
			if (string.IsNullOrWhiteSpace(model.CategoryId.ToString()))
			{
				throw new Exception("Vui lòng chọn thể loại.");
			}
			// Kiểm tra tên mới của sản phẩm có bị trùng tên sản phẩm khác
			bool isExistProductName = _unitOfWork.GetRepository<Product>().Entities.Any(p => p.Name == model.Name && !p.DeletedTime.HasValue && p.Id != id);
			if (isExistProductName)
			{
				throw new Exception("Sản phẩm khác có tên" + model.Name + " đã tồn tại.");
			}
			//Cập nhật và lưu sản phẩm vào db
			_mapper.Map(model, product);
			product.LastUpdateTime = CoreHelper.SystemTimeNows;
			_unitOfWork.GetRepository<Product>().Update(product);
			_unitOfWork.Save();
		}

		public void DeleteProduct(int id)
		{
			// Lấy sản phẩm - kiểm tra sự tồn tại
			Product product = _unitOfWork.GetRepository<Product>().Entities.FirstOrDefault(p => p.Id == id && !p.DeletedTime.HasValue) ?? throw new Exception("Không tìm thấy sản phẩm.");

			//Check id sản phẩm này có tồn tại trong Order ko -> nếu ko, xóa cứng
			bool isExistInAnyOrder = _unitOfWork.GetRepository<Order>().Entities.Any(o => o.ProductId == id);
			if (isExistInAnyOrder == false)
			{
				_unitOfWork.GetRepository<Product>().Delete(id);
				_unitOfWork.Save();
				return;
			}

			//Xóa mềm
			product.DeletedTime = CoreHelper.SystemTimeNows;
			_unitOfWork.GetRepository<Product>().Update(product);
			_unitOfWork.Save();
		}
	} 
}
