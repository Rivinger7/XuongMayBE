using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.Core;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMayBE.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}


		/// <summary>
		/// Lấy các sản phẩm không bị xóa, sort nếu muốn
		/// </summary>
		/// <param name="sortByName"></param>
		/// <returns></returns>
		[HttpGet("all_products")]
		public async Task<IActionResult> GetExistProducts(int pageNumber = 1, int pageSize = 3, bool? sortByName = null)
		{
			try
			{
				BasePaginatedList<ResponseProductModel> result = await _productService.GetProductsAsync(pageNumber, pageSize, sortByName);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy các sản phẩm theo tên và thể loại, nếu ko nhập gì -> in ra tất cả sản phẩm
		/// </summary>
		/// <param name="name"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		[HttpGet("search_products")]
		public async Task<IActionResult> SearchProducts(int pageNumber = 1, int pageSize = 3, string? name = null, string? category = null)
		{
			try
			{
				BasePaginatedList<ResponseProductModel> result = await _productService.SearchProductsAsync(pageNumber, pageSize, name, category);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy sản phẩm theo id
		/// </summary>
		[HttpGet("get_by_id")]
		public async Task<IActionResult> GetProductById(int id)
		{
			try
			{
				ResponseProductModel result = await _productService.GetProductAsync(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}


		/// <summary>
		/// Thêm sản phẩm mới, có tên khác với sản phẩm đã tồn tại
		/// </summary>
		[HttpPost("create")]
		public async Task<IActionResult> Add(CreateProductModel model)
		{
			try
			{
				ResponseProductModel result = await _productService.CreateProductAsync(model);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Cập nhật thông tin sản phẩm
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, CreateProductModel model)
		{
			try
			{
				ResponseProductModel result = await _productService.UpdateProductAsync(id, model);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Xóa 1 sản phẩm (thành công nếu sản phẩm này không thuộc order chưa hoàn thành)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("delete")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				ResponseProductModel result = await _productService.DeleteProductAsync(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new {Message = ex.Message});
			}
		}
	}
}
