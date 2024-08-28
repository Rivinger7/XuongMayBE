using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.WarehouseModelViews;
using static XuongMay.Core.Base.BaseException;

namespace XuongMayBE.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WarehouseController : Controller
	{
		private readonly IWarehouseService _warehouseService;
		public WarehouseController(IWarehouseService warehouseService)
		{
			_warehouseService = warehouseService;
		}

		[HttpPost("export")]
		public async Task<IActionResult> ExportProduct(ExportProductModel exportModel)
		{
			try
			{
				await _warehouseService.ExportProductAsync(exportModel);
				return Ok("Export successfully!");
			}
			catch (ErrorException eex)
			{
				return StatusCode(eex.StatusCode, eex.ErrorDetail.ErrorMessage);
			}
		}
	}
}
