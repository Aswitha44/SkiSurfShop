﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SkiSurf.API.Errors;
using SkiSurf.Core.Entities;
using SkiSurf.Core.Entities.OrderAggregate;
using SkiSurf.Core.Interfaces;
using Stripe;

namespace SkiSurf.API.Controllers
{
 
    public class PaymentsController : BaseApiController
    {
        private string? _whSecret;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _config;

        public PaymentsController(IPaymentService paymentService,ILogger<PaymentsController> logger,IConfiguration config) { 
         _paymentService = paymentService;
            _logger = logger;
            _config =config;
            _whSecret = _config["StripeSettings:WhSecret"];


        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket= await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket== null)
            {
                return BadRequest(new ApiResponse(400, "Problem with your basket"));

            }
            return basket;
        }


        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _whSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment succeeded:{intent.Id} ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    _logger.LogInformation("Order updated to payment received: {order.Id} ", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment failed:{intent.Id}  ", intent.Id);
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    _logger.LogInformation("Order updated to payment failed: {order.Id} ", order.Id);
                    break;
            }

            return new EmptyResult();
        }

    }
}
