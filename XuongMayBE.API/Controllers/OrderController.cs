﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
		/// Lấy danh sách đơn hàng
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <param name="isCompleted"></param>
		/// <param name="productName"></param>
		/// <returns></returns>
		[HttpGet("get_orders")]
		public async Task<IActionResult> GetAllOrder(int pageNumber = 1, int pageSize = 3, bool? isCompleted = null, string? productName = null)
		{
			try
			{
				var result = await _orderService.GetAllOrderAsync(pageNumber, pageSize, isCompleted, productName);
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
		public async Task<IActionResult> AddOrder(AddOrderModelView model)
		{
			try
			{
				var result = await _orderService.AddOrderAsync(model);
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
		public async Task<IActionResult> UpdateOrder(int id, UpdateOrderModelView model)
		{
			try
			{
				await _orderService.UpdateOrderAsync(id, model);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		/// <summary>
		/// Xóa đơn hàng
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteOrder(int id)
		{
			try
			{
				await _orderService.DeleteOrderAsync(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

	}
}
