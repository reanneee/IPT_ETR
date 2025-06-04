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
    public class CartController : Controller
    {
        private readonly DatabaseHelper _db;

        public CartController(DatabaseHelper db)
        {
            _db = db;
        }

        // SOLUTION 1: Ensure session is always created consistently
        private string GetOrCreateSessionId()
        {
            // First, try to get existing session
            string sessionId = HttpContext.Session.Id;

            // If session is empty or new, ensure it's created by setting a value
            if (string.IsNullOrEmpty(sessionId) || !HttpContext.Session.Keys.Any())
            {
                HttpContext.Session.SetString("CartSession", "initialized");
                sessionId = HttpContext.Session.Id;
            }

            return sessionId;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                var cartItems = GetUserCartItems(userId.Value);
                return View(cartItems);
            }
            else
            {
                var guestCartItems = GetGuestCartItems();

                // Enhanced debug information
                ViewBag.SessionId = GetOrCreateSessionId();
                ViewBag.CartItemsCount = guestCartItems?.Count ?? 0;
                ViewBag.GuestCartCount = GetGuestCartCountForSession();
                ViewBag.AllGuestCartCount = GetAllGuestCartCount();
                ViewBag.RawGuestCartData = GetRawGuestCartData();

                return View("GuestCart", guestCartItems);
            }
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                AddToUserCart(userId.Value, productId, quantity);
            }
            else
            {
                AddToGuestCart(productId, quantity);
            }

            TempData["Success"] = "Product added to cart!";

            // For AJAX requests, return JSON with cart count
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cartCount = userId.HasValue ? GetUserCartCount(userId.Value) : GetGuestCartCount();
                return Json(new
                {
                    success = true,
                    message = "Product added to cart!",
                    cartCount = cartCount
                });
            }

            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpPost]
        public IActionResult AddToCartAjax(int productId, int quantity = 1)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserID");

                if (userId.HasValue)
                {
                    AddToUserCart(userId.Value, productId, quantity);
                }
                else
                {
                    AddToGuestCart(productId, quantity);
                }

                var cartCount = userId.HasValue ? GetUserCartCount(userId.Value) : GetGuestCartCount();

                return Json(new
                {
                    success = true,
                    message = "Product added to cart!",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error adding product to cart: " + ex.Message
                });
            }
        }

        private void AddToUserCart(int userId, int productId, int quantity)
        {
            // Get or create cart
            var cartQuery = $"SELECT CartID FROM Cart WHERE UserID = {userId}";
            var cartResult = _db.SelectQuery(cartQuery);

            int cartId;
            if (cartResult.Rows.Count == 0)
            {
                var createCartQuery = $"INSERT INTO Cart (UserID) VALUES ({userId})";
                _db.ExecuteQuery(createCartQuery);

                var getCartIdQuery = "SELECT LAST_INSERT_ID() as CartID";
                var cartIdResult = _db.SelectQuery(getCartIdQuery);
                cartId = Convert.ToInt32(cartIdResult.Rows[0]["CartID"]);
            }
            else
            {
                cartId = Convert.ToInt32(cartResult.Rows[0]["CartID"]);
            }

            // Add item to cart
            var insertQuery = $@"INSERT INTO CartItems (CartID, ProductID, Quantity) 
                                VALUES ({cartId}, {productId}, {quantity})
                                ON DUPLICATE KEY UPDATE Quantity = Quantity + {quantity}";
            _db.ExecuteQuery(insertQuery);
        }

        private void AddToGuestCart(int productId, int quantity)
        {
            // FIXED: Use consistent session ID
            var sessionId = GetOrCreateSessionId();

            var insertQuery = $@"INSERT INTO GuestCart (SessionID, ProductID, Quantity) 
                                VALUES ('{sessionId}', {productId}, {quantity})
                                ON DUPLICATE KEY UPDATE Quantity = Quantity + {quantity}";
            _db.ExecuteQuery(insertQuery);
        }

        private List<GuestCartItem> GetGuestCartItems()
        {
            // FIXED: Use consistent session ID
            var sessionId = GetOrCreateSessionId();

            if (string.IsNullOrEmpty(sessionId))
            {
                return new List<GuestCartItem>();
            }

            var query = $@"SELECT gc.*, p.ProductName, p.Price, p.Image 
                          FROM GuestCart gc
                          JOIN Products p ON gc.ProductID = p.ProductID
                          WHERE gc.SessionID = '{sessionId}'";

            try
            {
                var dt = _db.SelectQuery(query);
                var items = new List<GuestCartItem>();

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new GuestCartItem
                    {
                        SessionID = row["SessionID"].ToString(),
                        ProductID = Convert.ToInt32(row["ProductID"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        ProductName = row["ProductName"]?.ToString() ?? "Unknown Product",
                        Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0,
                        Image = row["Image"]?.ToString() ?? ""
                    });
                }

                return items;
            }
            catch (Exception ex)
            {
                ViewBag.JoinError = ex.Message;
                return new List<GuestCartItem>();
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                UpdateUserCartQuantity(userId.Value, productId, quantity);
            }
            else
            {
                UpdateGuestCartQuantity(productId, quantity);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                RemoveFromUserCart(userId.Value, productId);
            }
            else
            {
                RemoveFromGuestCart(productId);
            }

            return RedirectToAction("Index");
        }

        private void UpdateUserCartQuantity(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveFromUserCart(userId, productId);
                return;
            }

            var query = $@"UPDATE CartItems ci
                          JOIN Cart c ON ci.CartID = c.CartID
                          SET ci.Quantity = {quantity}
                          WHERE c.UserID = {userId} AND ci.ProductID = {productId}";
            _db.ExecuteQuery(query);
        }

        private void UpdateGuestCartQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveFromGuestCart(productId);
                return;
            }

            // FIXED: Use consistent session ID
            var sessionId = GetOrCreateSessionId();
            var query = $@"UPDATE GuestCart 
                          SET Quantity = {quantity}
                          WHERE SessionID = '{sessionId}' AND ProductID = {productId}";
            _db.ExecuteQuery(query);
        }

        private void RemoveFromUserCart(int userId, int productId)
        {
            var query = $@"DELETE ci FROM CartItems ci
                          JOIN Cart c ON ci.CartID = c.CartID
                          WHERE c.UserID = {userId} AND ci.ProductID = {productId}";
            _db.ExecuteQuery(query);
        }

        private void RemoveFromGuestCart(int productId)
        {
            // FIXED: Use consistent session ID
            var sessionId = GetOrCreateSessionId();
            var query = $@"DELETE FROM GuestCart 
                          WHERE SessionID = '{sessionId}' AND ProductID = {productId}";
            _db.ExecuteQuery(query);
        }

        // Debug helper methods (updated to use consistent session ID)
        private int GetGuestCartCountForSession()
        {
            var sessionId = GetOrCreateSessionId();
            if (string.IsNullOrEmpty(sessionId)) return 0;

            try
            {
                var query = $"SELECT COUNT(*) as Count FROM GuestCart WHERE SessionID = '{sessionId}'";
                var result = _db.SelectQuery(query);
                return result.Rows.Count > 0 ? Convert.ToInt32(result.Rows[0]["Count"]) : 0;
            }
            catch
            {
                return 0;
            }
        }

        private int GetAllGuestCartCount()
        {
            try
            {
                var query = "SELECT COUNT(*) as Count FROM GuestCart";
                var result = _db.SelectQuery(query);
                return result.Rows.Count > 0 ? Convert.ToInt32(result.Rows[0]["Count"]) : 0;
            }
            catch
            {
                return 0;
            }
        }

        private List<object> GetRawGuestCartData()
        {
            var sessionId = GetOrCreateSessionId();
            var data = new List<object>();

            try
            {
                var query = $"SELECT * FROM GuestCart WHERE SessionID = '{sessionId}'";
                var dt = _db.SelectQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        SessionID = row["SessionID"].ToString(),
                        ProductID = row["ProductID"].ToString(),
                        Quantity = row["Quantity"].ToString(),
                        CreatedAt = row["CreatedAt"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                data.Add(new { Error = ex.Message });
            }

            return data;
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


        private List<object> GetProductsData()
        {
            var data = new List<object>();

            try
            {
                var query = "SELECT ProductID, ProductName, Price FROM Products LIMIT 5";
                var dt = _db.SelectQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    data.Add(new
                    {
                        ProductID = row["ProductID"].ToString(),
                        ProductName = row["ProductName"].ToString(),
                        Price = row["Price"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                data.Add(new { Error = ex.Message });
            }

            return data;
        }

       

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            int count = 0;

            if (userId.HasValue)
            {
                count = GetUserCartCount(userId.Value);
            }
            else
            {
                count = GetGuestCartCount();
            }

            return Json(new { count = count });
        }

        // Add these helper methods to your CartController

        private int GetUserCartCount(int userId)
        {
            try
            {
                var query = $@"SELECT SUM(ci.Quantity) as TotalCount 
                      FROM CartItems ci
                      JOIN Cart c ON ci.CartID = c.CartID
                      WHERE c.UserID = {userId}";

                var result = _db.SelectQuery(query);

                if (result.Rows.Count > 0 && result.Rows[0]["TotalCount"] != DBNull.Value)
                {
                    return Convert.ToInt32(result.Rows[0]["TotalCount"]);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private int GetGuestCartCount()
        {
            try
            {
                var sessionId = GetOrCreateSessionId();
                if (string.IsNullOrEmpty(sessionId)) return 0;

                var query = $@"SELECT SUM(Quantity) as TotalCount 
                      FROM GuestCart 
                      WHERE SessionID = '{sessionId}'";

                var result = _db.SelectQuery(query);

                if (result.Rows.Count > 0 && result.Rows[0]["TotalCount"] != DBNull.Value)
                {
                    return Convert.ToInt32(result.Rows[0]["TotalCount"]);
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

    }

}