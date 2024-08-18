using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
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
		/// Lấy toàn bộ sản phẩm, kể cả những cái đã xóa
		/// </summary>
		[HttpGet("all_products")]
		public async Task<IActionResult> GetAllProducts()
		{
			try
			{
				IEnumerable<ResponseProductModel> result = await _productService.GetAsync();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy các sản phẩm không bị xóa, sort nếu muốn
		/// </summary>
		/// <param name="sortByName"></param>
		/// <returns></returns>
		[HttpGet("exist_products")]
		public async Task<IActionResult> GetExistProducts(bool? sortByName)
		{
			try
			{
				IEnumerable<ResponseProductModel> result = await _productService.GetProductsAsync(sortByName);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Thêm sản phẩm mới
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
		/// Xóa sản phẩm
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("delete")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _productService.DeleteProductAsync(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new {Message = ex.Message});
			}
		}
	}
}
