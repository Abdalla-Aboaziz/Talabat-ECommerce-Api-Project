using ECommerce.ServiceAbstraction;
using ECommerce.Shared.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presentation.Controllers
{
    public class OrderController: ApiBaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
           _orderService = orderService;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var result= await _orderService.CreateOrderAsync( orderDto, Email!);
            return HandelResult(result);

        }

        [HttpGet("deliveryMethods")]
        public async Task<IActionResult> GetAllDeliveryMethods()
        {
            var result = await _orderService.GetAllDeliveryMethodsAsync();
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrderForSpecificUser()
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _orderService.GetOrdersForUserAsync(Email!);
            return Ok(result);

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForSpecificUser(Guid id)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _orderService.GetOrderByIdAsync(id, Email!);
            return HandelResult(result);

        }






        }
}
