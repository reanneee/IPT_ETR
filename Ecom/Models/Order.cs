using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public string ProductName { get; set; }
    }

    /*namespace Ecom.Models
    {
        public class Order
        {
            public int OrderID { get; set; }
            public int UserID { get; set; }
            public DateTime OrderDate { get; set; }
            public string Status { get; set; }
            public decimal TotalAmount { get; set; }
            public string ShippingAddress { get; set; }
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDate { get; set; }
            public DateTime? DeliveredDate { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
            public List<OrderTrackingEvent> TrackingTimeline { get; set; } = new List<OrderTrackingEvent>();

            // Helper properties
            public bool CanConfirmDelivery => Status?.ToLower() == "shipped";
            public string StatusBadgeClass => Status?.ToLower() switch
            {
                "pending" => "badge-warning",
                "processing" => "badge-info",
                "shipped" => "badge-primary",
                "delivered" => "badge-success",
                "cancelled" => "badge-danger",
                _ => "badge-secondary"
            };
        }

        public class OrderItem
        {
            public int OrderItemID { get; set; }
            public int OrderID { get; set; }
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public decimal PriceAtPurchase { get; set; }
            public string ProductName { get; set; }
            public string ProductImage { get; set; }
            public decimal Total => Quantity * PriceAtPurchase;
        }

        public class OrderTrackingEvent
        {
            public string Status { get; set; }
            public string Description { get; set; }
            public DateTime? Date { get; set; }
            public bool IsCompleted { get; set; }
            public string Icon { get; set; }
        }
    }*/

}
