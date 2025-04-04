﻿using Microsoft.Extensions.Configuration;
using SkiSurf.Core.Entities;
using SkiSurf.Core.Entities.OrderAggregate;
using SkiSurf.Core.Interfaces;
using SkiSurf.Core.Specifications;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = SkiSurf.Core.Entities.Product;

namespace SkiSurf.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IConfiguration config)
        {
           _unitOfWork = unitOfWork;
            _config = config;           
            _basketRepository = basketRepository;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null) {
                return null;
            }
            var shippingPrice = 0m;
            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().
                    GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach(var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) +
                  (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) +
                  (long)shippingPrice * 100
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);

            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;

        }

       
        async Task<Order> IPaymentService.UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.Complete();

            return order;
        }

        async Task<Order> IPaymentService.UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentReceived;
            await _unitOfWork.Complete();

            return order;
        }
    }
}
