// Add these model classes to your Ecom.Models namespace

using System;
using System.ComponentModel.DataAnnotations;

namespace Ecom.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        // Navigation/Display properties
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal LineTotal => Price * Quantity;
    }

    public class OrderSummary
    {
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax => SubTotal * 0.12m; // 12% tax rate
        public decimal ShippingFee { get; set; } = 50.00m; // Default shipping fee
        public decimal GrandTotal => SubTotal + Tax + ShippingFee;
    }

   
}