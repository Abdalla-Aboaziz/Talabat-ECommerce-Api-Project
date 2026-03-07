using ECommerce.ServiceAbstraction;
using ECommerce.Shared.BasketDtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presentation.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
           _paymentService = paymentService;
        }
        // Create PaymentIntent
        [HttpPost("{BasketId}")]
        public async Task<ActionResult<BasketDtos>> CreatePaymentIntent(string BasketId)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(BasketId);
           return HandelResult(result);
        }
    }
}
