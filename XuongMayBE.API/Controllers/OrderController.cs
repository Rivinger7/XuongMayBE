using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XuongMay.Contract.Services.Interface;
using XuongMay.ModelViews.OrderModelViews;

namespace XuongMayBE.API.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		/// <summary>
		/// Lấy toàn bộ đơn hàng
		/// </summary>
		/// <param name="searcProductName"></param>
		/// <returns></returns>
		[HttpGet("all_order")]
		public IActionResult GetAllOrder(string? searcProductName)
		{
			try
			{
				var result = _orderService.GetAllOrder(searcProductName);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Thêm đơn hàng
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost("create")]
		public IActionResult AddOrder(AddOrderModelView model)
		{
			try
			{
				var result = _orderService.AddOrder(model);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Cập nhật thông tin đơn hàng
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut("update")]
		public IActionResult UpdateOrder(int id, UpdateOrderModelView model)
		{
			try
			{
				_orderService.UpdateOrder(id, model);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		[HttpDelete("delete")]
		public IActionResult DeleteOrder(int id)
		{
			try
			{
				_orderService.DeleteOrder(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}
	}
}
