using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.ChamberModelViews;
using XuongMay.ModelViews.WarehouseModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMayBE.API.Controllers
{
	//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Manager")]
	[Route("api/[controller]")]
	[ApiController]
	public class WarehouseController : Controller
	{
		private readonly IChamberService _chamberService;

		public WarehouseController(IChamberService chamberService)
		{
			_chamberService = chamberService;
		}

		/// <summary>
		/// Nhập kho
		/// </summary>
		[HttpPost("import")]
		public async Task<IActionResult> Import(ImportModel model)
		{
			try
			{
				await _chamberService.ImportChambers(model);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Lấy danh sách lịch sử nhập/xuất kho của 1 khoang
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <param name="chamberID"></param>
		/// <param name="searchId"></param>
		/// <param name="searchProductName"></param>
		/// <param name="importAndExport"></param>
		/// <returns></returns>
		[HttpGet("{chamberID}/get_inventory_history")]
		public async Task<IActionResult> GetInventoryHistories(int pageNumber = 1, int pageSize = 3, int chamberID = 1, int? searchId = null, string? searchProductName = null, bool? importAndExport = null)
		{
			try
			{
				var result = await _chamberService.GetInventoryHistoriesAsync(pageNumber, pageSize, chamberID, searchId, searchProductName, importAndExport);
				return Ok(result);
			}
			catch (Exception ex)

			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Xuất kho
		/// </summary>
		/// <param name="exportModel"></param>
		/// <returns></returns>
		[HttpPost("export")]
		public async Task<IActionResult> ExportProduct(ExportProductModel exportModel)
		{
			try
			{
				await _chamberService.ExportProductAsync(exportModel);
				return Ok("Export successfully!");
			}
			catch (ErrorException eex)
			{
				return StatusCode(eex.StatusCode, eex.ErrorDetail.ErrorMessage);
			}
		}

		/// <summary>
		/// Cập nhật sản phẩm hiện tại sang 1 sản phẩm khác
		/// </summary>
		/// <param name="chamberId"></param>
		/// <param name="productId"></param>
		/// <param name="productIdNew"></param>
		/// <param name="itemPerBox"></param>
		/// <returns></returns>
		[HttpPut("update")]
		public async Task<IActionResult> UpdateProduct(int chamberId, int productId, int productIdNew, int itemPerBox)
		{
			try
			{
				await _chamberService.UpdateProduct(chamberId, productId, productIdNew, itemPerBox);
				return Ok();
			}
			catch (ErrorException ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}
	}
}

