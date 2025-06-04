using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.Models
{
    // Update your Order model to include these properties
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Additional properties for admin view
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string UserAddress { get; set; }
    }

    // Update your OrderItem model to include these properties
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public string ProductName { get; set; }

        // Additional properties for detailed view
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }

        // Calculated property
        public decimal Total => Quantity * PriceAtPurchase;
    }



}
