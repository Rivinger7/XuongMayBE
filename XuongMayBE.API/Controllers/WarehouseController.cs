using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.ChamberModelViews;
using XuongMay.ModelViews.ProductModelViews;
using XuongMay.Services.Service;

namespace XuongMayBE.API.Controllers
{
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
	}
}
