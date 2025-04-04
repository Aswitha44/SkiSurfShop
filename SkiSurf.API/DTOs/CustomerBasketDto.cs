﻿using SkiSurf.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace SkiSurf.API.DTOs
{
    public class CustomerBasketDto
    {
        public CustomerBasketDto(string id)
        {
            Id = id;
        }
        [Required]
        public string Id { get; set; }
        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        public int? DeliveryMethodId { get; set; }
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
