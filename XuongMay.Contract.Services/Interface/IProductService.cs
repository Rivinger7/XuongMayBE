using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Contract.Repositories.Interface;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IProductService
	{
		public List<ResponseProductModel> Get();
		public List<ResponseProductModel> GetProducts(bool? sortByName);
		public ResponseProductModel CreateProduct(CreateProductModel model);
		public void UpdateProduct(int id, CreateProductModel model);
		public void DeleteProduct(int id);
	}
}
