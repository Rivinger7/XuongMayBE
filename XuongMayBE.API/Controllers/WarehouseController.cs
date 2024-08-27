using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;

namespace XuongMayBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WarehouseController : ControllerBase
	{
		private readonly IChamberService _chamberService;

		public WarehouseController(IChamberService chamberService)
		{
			_chamberService = chamberService;
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
			catch(Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}
	}
}
