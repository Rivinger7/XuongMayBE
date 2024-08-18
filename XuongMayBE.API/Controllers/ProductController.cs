using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.ProductModelViews;

namespace XuongMayBE.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
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
		public IActionResult GetAllProducts()
		{
			try
			{
				List<ResponseProductModel> result = _productService.Get();
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
		public IActionResult GetExistProducts(bool? sortByName)
		{
			try
			{
				List<ResponseProductModel> result = _productService.GetProducts(sortByName);
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
		public IActionResult Add(CreateProductModel model)
		{
			try
			{
				ResponseProductModel result = _productService.CreateProduct(model);
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
		[HttpPut]
		[Route("/api/[controller]/update/{id}")]
		public IActionResult Update(int id, CreateProductModel model)
		{
			try
			{
				_productService.UpdateProduct(id, model);
				return Ok();
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
		[HttpDelete("Delete")]
		public IActionResult Delete(int id)
		{
			try
			{
				_productService.DeleteProduct(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new {Message = ex.Message});
			}
		}
	}
}
