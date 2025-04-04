﻿using System.ComponentModel.DataAnnotations;

namespace SkiSurf.API.DTOs
{
    public class BasketItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public required string ProductName { get; set; }
        [Required]
        [Range(0.10, double.MaxValue, ErrorMessage ="Price must be greater than 0")]
        public decimal Price { get; set; }
        [Required]
        [Range(1,double.MaxValue, ErrorMessage = "Quantity must be atleast 1")]
        public int Quantity { get; set; }
        [Required]
        public required string PictureUrl { get; set; }
        [Required]
        public required string Brand { get; set; }
        [Required]
        public required string Type { get; set; }

    }
}
