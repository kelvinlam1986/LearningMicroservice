﻿using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.Baskets
{
    public class CartItemDto
    {
        [Required]
        [Range(1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}")]
        public int Quantity { get; set; }
        [Required]
        [Range(0.1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}")]
        public int ItemPrice { get; set; }

        [Required]
        public string ItemNo { get; set; }

        [Required]
        public string ItemName { get; set; }

        public int AvailableQuantity { get; set; }

        public void SetAvailableQuantity(int quantity)
        {
            AvailableQuantity = quantity;
        }
    }
}
