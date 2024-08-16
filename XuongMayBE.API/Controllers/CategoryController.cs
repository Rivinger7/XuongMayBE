using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.CategoryModels;

namespace XuongMayBE.API.Controllers
{
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
		[HttpGet]
		public IActionResult GetAllCategory(bool? sortByName)
		{
			try
			{
				var result = _categoryService.GetAllCategory(sortByName);
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
		[HttpPost]
		public IActionResult Add(AddCategoryModel model)
		{
			try
			{
				var result = _categoryService.Add(model);
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
		[HttpPut]
		[Route("/api/[controller]/{id}")]
		public IActionResult Update(int id, AddCategoryModel model)
		{
			try
			{
				_categoryService.Update(id, model);
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
		[HttpDelete]
		public IActionResult Delete(int id)
		{
			try
			{
				_categoryService.Delete(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new {Message = ex.Message});
			}
		}
	}
}
