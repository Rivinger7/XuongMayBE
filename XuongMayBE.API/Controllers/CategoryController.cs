using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.CategoryModels;

namespace XuongMayBE.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		/// <summary>
		/// Lấy toàn bộ danh mục
		/// </summary>
		/// <param name="sortByName"></param>
		/// <returns></returns>
		[HttpGet("all_category")]
		public async Task<IActionResult> GetAllCategory(int? id, bool? sortByName, int pageNumber = 1, int pageSize = 3)
		{
			try
			{
				var result = await _categoryService.GetAllCategoryAsync(id, sortByName, pageNumber, pageSize);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Thêm danh mục mới
		/// </summary>
		[HttpPost("create")]
		public async Task<IActionResult> Add(AddCategoryModel model)
		{
			try
			{
				var result = await _categoryService.AddAsync(model);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Cập nhật thông tin danh mục
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, AddCategoryModel model)
		{
			try
			{
				await _categoryService.UpdateAsync(id, model);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Xóa danh mục
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("delete")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _categoryService.DeleteAsync(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new {Message = ex.Message});
			}
		}
	}
}
