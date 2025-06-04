using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ecom.DataAccess;
using Ecom.Models;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace Ecom.Controllers
{
    public class OrderController : Controller
    {
        private readonly DatabaseHelper _db;

        public OrderController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = GetUserOrders(userId.Value);
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = GetOrderDetails(id, userId.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = GetUserCartItems(userId.Value);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Checkout(string shippingAddress)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                ModelState.AddModelError("", "Shipping address is required");
                var cartItems = GetUserCartItems(userId.Value);
                return View(cartItems);
            }

            var orderId = CreateOrder(userId.Value, shippingAddress);
            if (orderId > 0)
            {
                ClearUserCart(userId.Value);
                TempData["Success"] = "Order placed successfully!";
                return RedirectToAction("Details", new { id = orderId });
            }

            TempData["Error"] = "Failed to place order";
            return View();
        }

        private List<Order> GetUserOrders(int userId)
        {
            var query = $@"SELECT * FROM Orders 
                          WHERE UserID = {userId} 
                          ORDER BY OrderDate DESC";

            var dt = _db.SelectQuery(query);
            var orders = new List<Order>();

            foreach (DataRow row in dt.Rows)
            {
                orders.Add(new Order
                {
                    OrderID = Convert.ToInt32(row["OrderID"]),
                    UserID = Convert.ToInt32(row["UserID"]),
                    OrderDate = Convert.ToDateTime(row["OrderDate"]),
                    Status = row["Status"].ToString(),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    ShippingAddress = row["ShippingAddress"].ToString()
                });
            }

            return orders;
        }

        private Order GetOrderDetails(int orderId, int userId)
        {
            var orderQuery = $@"SELECT * FROM Orders 
                               WHERE OrderID = {orderId} AND UserID = {userId}";

            var orderResult = _db.SelectQuery(orderQuery);
            if (orderResult.Rows.Count == 0) return null;

            var orderRow = orderResult.Rows[0];
            var order = new Order
            {
                OrderID = Convert.ToInt32(orderRow["OrderID"]),
                UserID = Convert.ToInt32(orderRow["UserID"]),
                OrderDate = Convert.ToDateTime(orderRow["OrderDate"]),
                Status = orderRow["Status"].ToString(),
                TotalAmount = Convert.ToDecimal(orderRow["TotalAmount"]),
                ShippingAddress = orderRow["ShippingAddress"].ToString()
            };

            // Get order items
            var itemsQuery = $@"SELECT oi.*, p.ProductName 
                               FROM OrderItems oi
                               JOIN Products p ON oi.ProductID = p.ProductID
                               WHERE oi.OrderID = {orderId}";

            var itemsResult = _db.SelectQuery(itemsQuery);
            foreach (DataRow itemRow in itemsResult.Rows)
            {
                order.OrderItems.Add(new OrderItem
                {
                    OrderItemID = Convert.ToInt32(itemRow["OrderItemID"]),
                    OrderID = Convert.ToInt32(itemRow["OrderID"]),
                    ProductID = Convert.ToInt32(itemRow["ProductID"]),
                    Quantity = Convert.ToInt32(itemRow["Quantity"]),
                    PriceAtPurchase = Convert.ToDecimal(itemRow["PriceAtPurchase"]),
                    ProductName = itemRow["ProductName"].ToString()
                });
            }

            return order;
        }

        private List<CartItem> GetUserCartItems(int userId)
        {
            var query = $@"SELECT ci.*, p.ProductName, p.Price, p.Image 
                          FROM CartItems ci
                          JOIN Cart c ON ci.CartID = c.CartID
                          JOIN Products p ON ci.ProductID = p.ProductID
                          WHERE c.UserID = {userId}";

            var dt = _db.SelectQuery(query);
            var items = new List<CartItem>();

            foreach (DataRow row in dt.Rows)
            {
                items.Add(new CartItem
                {
                    CartItemID = Convert.ToInt32(row["CartItemID"]),
                    CartID = Convert.ToInt32(row["CartID"]),
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ProductName = row["ProductName"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Image = row["Image"].ToString()
                });
            }

            return items;
        }

        private int CreateOrder(int userId, string shippingAddress)
        {
            var cartItems = GetUserCartItems(userId);
            if (!cartItems.Any()) return 0;

            var totalAmount = cartItems.Sum(item => item.Total);

            // Create order
            var createOrderQuery = $@"INSERT INTO Orders (UserID, TotalAmount, ShippingAddress) 
                                     VALUES ({userId}, {totalAmount}, '{shippingAddress}')";
            _db.ExecuteQuery(createOrderQuery);

            // Get order ID
            var getOrderIdQuery = "SELECT LAST_INSERT_ID() as OrderID";
            var orderIdResult = _db.SelectQuery(getOrderIdQuery);
            var orderId = Convert.ToInt32(orderIdResult.Rows[0]["OrderID"]);

            // Create order items
            foreach (var item in cartItems)
            {
                var createItemQuery = $@"INSERT INTO OrderItems (OrderID, ProductID, Quantity, PriceAtPurchase) 
                                        VALUES ({orderId}, {item.ProductID}, {item.Quantity}, {item.Price})";
                _db.ExecuteQuery(createItemQuery);

                // Update product stock
                var updateStockQuery = $@"UPDATE Products 
                                         SET Stock = Stock - {item.Quantity} 
                                         WHERE ProductID = {item.ProductID}";
                _db.ExecuteQuery(updateStockQuery);
            }

            return orderId;
        }

        private void ClearUserCart(int userId)
        {
            var query = $@"DELETE ci FROM CartItems ci
                          JOIN Cart c ON ci.CartID = c.CartID
                          WHERE c.UserID = {userId}";
            _db.ExecuteQuery(query);
        }
    }
}
