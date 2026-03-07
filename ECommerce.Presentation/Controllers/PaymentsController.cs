using ECommerce.ServiceAbstraction;
using ECommerce.Shared.BasketDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
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
        //stripe listen --forward-to localhost:7097/api/Payments/webhook
        [AllowAnonymous]
        [Route("webhook")]
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            const string endpointSecret = "whsec_6ff22f85d9658850e2ac72b055edc92d1b50dc2a2b911624bb5729f426980a8c";
           
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);

                // If on SDK version < 46, use class Events instead of EventTypes
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent);
                }
                else if (stripeEvent.Type == EventTypes.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
          
        }
    }
}
