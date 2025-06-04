using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ecom.DataAccess;
using Ecom.Models;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ecom.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(DatabaseHelper db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.TotalProducts = GetTotalProducts();
            ViewBag.TotalOrders = GetTotalOrders();
            ViewBag.TotalUsers = GetTotalUsers();
            ViewBag.TotalRevenue = GetTotalRevenue();
            ViewBag.TopSellingProducts = GetTopSellingProducts();

            return View();
        }

        private int GetTotalProducts()
        {
            var query = "SELECT COUNT(*) as Count FROM Products WHERE Visible = TRUE";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]);
        }

        private int GetTotalOrders()
        {
            var query = "SELECT COUNT(*) as Count FROM Orders";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]);
        }

        private int GetTotalUsers()
        {
            var query = "SELECT COUNT(*) as Count FROM Users WHERE Role = 'Customer'";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]);
        }

        private decimal GetTotalRevenue()
        {
            var query = "SELECT COALESCE(SUM(TotalAmount), 0) as Revenue FROM Orders WHERE Status = 'Delivered'";
            var result = _db.SelectQuery(query);
            return Convert.ToDecimal(result.Rows[0]["Revenue"]);
        }

        private List<TopSellingProduct> GetTopSellingProducts()
        {
            var query = @"
                SELECT 
                    p.ProductID,
                    p.ProductName,
                    p.Description,
                    p.Price,
                    p.Stock,
                    p.Image,
                    c.CategoryName,
                    COALESCE(SUM(oi.Quantity), 0) as TotalSold,
                    COALESCE(SUM(oi.Quantity * oi.PriceAtPurchase), 0) as TotalRevenue
                FROM Products p
                LEFT JOIN OrderItems oi ON p.ProductID = oi.ProductID
                LEFT JOIN Orders o ON oi.OrderID = o.OrderID AND o.Status = 'Delivered'
                LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                WHERE p.Visible = TRUE
                GROUP BY p.ProductID, p.ProductName, p.Description, p.Price, p.Stock, p.Image, c.CategoryName
                ORDER BY TotalSold DESC
                LIMIT 4";

            var result = _db.SelectQuery(query);
            var topProducts = new List<TopSellingProduct>();

            foreach (DataRow row in result.Rows)
            {
                topProducts.Add(new TopSellingProduct
                {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    ProductName = row["ProductName"].ToString(),
                    Description = row["Description"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    Image = row["Image"].ToString(),
                    CategoryName = row["CategoryName"].ToString(),
                    TotalSold = Convert.ToInt32(row["TotalSold"]),
                    TotalRevenue = Convert.ToDecimal(row["TotalRevenue"])
                });
            }

            return topProducts;
        }

        // User Management
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var users = GetAllUsers();
            return View(users);
        }

        public IActionResult Customers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var users = GetAllUsers().Where(u => u.Role == "Customer").ToList();
            return View(users);
        }


        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (EmailExists(user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(user);
                }

                // Hash password
                string hashedPassword = HashPassword(user.Password);

                var query = $@"INSERT INTO Users (Firstname, Middlename, Lastname, Address, Role, Email, Password, CreatedAt) 
                              VALUES ('{user.Firstname}', '{user.Middlename ?? ""}', '{user.Lastname}', 
                                     '{user.Address}', '{user.Role}', '{user.Email}', '{hashedPassword}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}')";

                var result = _db.ExecuteQuery(query);
                if (result > 0)
                {
                    TempData["Success"] = "User created successfully!";
                    return RedirectToAction("Users");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create user. Please try again.");
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = GetUserById(id);
            if (user == null) return NotFound();

            // Don't send password to view
            user.Password = "";
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Remove password validation for edit
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                // Check if email already exists for other users
                if (EmailExistsForOtherUser(user.Email, user.UserID))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(user);
                }

                var query = $@"UPDATE Users 
                              SET Firstname = '{user.Firstname}', 
                                  Middlename = '{user.Middlename ?? ""}', 
                                  Lastname = '{user.Lastname}', 
                                  Address = '{user.Address}', 
                                  Role = '{user.Role}', 
                                  Email = '{user.Email}'
                              WHERE UserID = {user.UserID}";

                var result = _db.ExecuteQuery(query);
                if (result > 0)
                {
                    TempData["Success"] = "User updated successfully!";
                    return RedirectToAction("Users");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update user. Please try again.");
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = GetUserById(id);
            if (user == null) return NotFound();

            var model = new ResetPasswordViewModel
            {
                UserID = id,
                UserName = $"{user.Firstname} {user.Lastname}"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                string hashedPassword = HashPassword(model.NewPassword);
                var query = $"UPDATE Users SET Password = '{hashedPassword}' WHERE UserID = {model.UserID}";

                var result = _db.ExecuteQuery(query);
                if (result > 0)
                {
                    TempData["Success"] = "Password reset successfully!";
                    return RedirectToAction("Users");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to reset password. Please try again.");
                }
            }

            // If we got this far, something failed, reload user info
            var user = GetUserById(model.UserID);
            if (user != null)
            {
                model.UserName = $"{user.Firstname} {user.Lastname}";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Get current admin user ID to prevent self-deletion
            var currentUserId = HttpContext.Session.GetInt32("UserID");
            if (currentUserId == id)
            {
                TempData["Error"] = "You cannot delete your own account!";
                return RedirectToAction("Users");
            }

            // Check if user has orders
            var orderCheckQuery = $"SELECT COUNT(*) as OrderCount FROM Orders WHERE UserID = {id}";
            var orderResult = _db.SelectQuery(orderCheckQuery);
            int orderCount = Convert.ToInt32(orderResult.Rows[0]["OrderCount"]);

            if (orderCount > 0)
            {
                TempData["Error"] = "Cannot delete user with existing orders. Consider deactivating the account instead.";
                return RedirectToAction("Users");
            }

            var query = $"DELETE FROM Users WHERE UserID = {id}";
            var result = _db.ExecuteQuery(query);

            if (result > 0)
            {
                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete user. Please try again.";
            }

            return RedirectToAction("Users");
        }

        // Product Management
        public IActionResult Products()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var products = GetAllProducts();
            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.Categories = GetCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                string imagePath = await SaveImageFile(imageFile);

                var query = $@"INSERT INTO Products (ProductName, Description, Price, Stock, CategoryID, Image) 
                              VALUES ('{product.ProductName}', '{product.Description}', {product.Price}, 
                                     {product.Stock}, {product.CategoryID}, '{imagePath}')";

                var result = _db.ExecuteQuery(query);
                if (result > 0)
                {
                    TempData["Success"] = "Product created successfully!";
                    return RedirectToAction("Products");
                }
            }

            ViewBag.Categories = GetCategories();
            return View(product);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = GetProductById(id);
            if (product == null) return NotFound();

            ViewBag.Categories = GetCategories();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                string imagePath = product.Image; // Keep existing image by default

                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(product.Image))
                    {
                        DeleteImageFile(product.Image);
                    }

                    // Save new image
                    imagePath = await SaveImageFile(imageFile);
                }

                var query = $@"UPDATE Products 
                              SET ProductName = '{product.ProductName}', 
                                  Description = '{product.Description}', 
                                  Price = {product.Price}, 
                                  Stock = {product.Stock}, 
                                  CategoryID = {product.CategoryID}, 
                                  Image = '{imagePath}',
                                  Visible = {(product.Visible ? 1 : 0)}
                              WHERE ProductID = {product.ProductID}";

                var result = _db.ExecuteQuery(query);
                if (result > 0)
                {
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction("Products");
                }
            }

            ViewBag.Categories = GetCategories();
            return View(product);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Get product to delete its image
            var product = GetProductById(id);
            if (product != null && !string.IsNullOrEmpty(product.Image))
            {
                DeleteImageFile(product.Image);
            }

            var query = $"UPDATE Products SET Visible = FALSE WHERE ProductID = {id}";
            _db.ExecuteQuery(query);

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Products");
        }

        // Image handling methods
        private async Task<string> SaveImageFile(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            // Create uploads directory if it doesn't exist
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsDir);

            // Generate unique filename
            string fileExtension = Path.GetExtension(imageFile.FileName);
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(uploadsDir, fileName);

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowedExtensions.Contains(fileExtension.ToLower()))
            {
                throw new InvalidOperationException("Invalid file type. Only image files are allowed.");
            }

            // Validate file size (5MB max)
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("File size must be less than 5MB.");
            }

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Return relative path for storing in database
            return $"/uploads/products/{fileName}";
        }

        private void DeleteImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw - image deletion shouldn't break the operation
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }

        // Order Management
        public IActionResult Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var orders = GetAllOrders();
            return View(orders);
        }

        // Replace the existing UpdateOrderStatus method in AdminController with this improved version
[HttpPost]
public IActionResult UpdateOrderStatus(int orderId, string status)
{
    if (!IsAdmin()) return RedirectToAction("Login", "Account");

    try
    {
        // Get the current order status before updating
        var currentOrderQuery = $"SELECT Status FROM Orders WHERE OrderID = {orderId}";
        var currentResult = _db.SelectQuery(currentOrderQuery);

        if (currentResult.Rows.Count == 0)
        {
            TempData["Error"] = "Order not found.";
            return RedirectToAction("Orders");
        }

        string currentStatus = currentResult.Rows[0]["Status"].ToString();

        // Handle stock restoration when cancelling an order
        if (status == "Cancelled" && currentStatus != "Cancelled")
        {
            RestoreOrderStock(orderId);
            TempData["Success"] = "Order cancelled successfully! Product stock has been restored.";
        }
        // Handle stock reduction when un-cancelling an order (changing from Cancelled to another status)
        else if (currentStatus == "Cancelled" && status != "Cancelled")
        {
            if (!ReduceOrderStock(orderId))
            {
                TempData["Error"] = "Cannot change order status: Insufficient stock for some products.";
                return RedirectToAction("Orders");
            }
            TempData["Success"] = "Order status updated successfully! Product stock has been adjusted.";
        }
        else
        {
            TempData["Success"] = "Order status updated successfully!";
        }

        // Prepare the update query with date handling
        string updateQuery;
        
        if (status == "Shipped")
        {
            // Set ShippedDate to current datetime when status is changed to Shipped
            updateQuery = $"UPDATE Orders SET Status = '{status}', ShippedDate = NOW() WHERE OrderID = {orderId}";
        }
        else if (status == "Delivered")
        {
            // Set DeliveredDate to current datetime when status is changed to Delivered
            // Also ensure ShippedDate is set if it wasn't already
            updateQuery = $@"UPDATE Orders 
                            SET Status = '{status}', 
                                DeliveredDate = NOW(),
                                ShippedDate = COALESCE(ShippedDate, NOW())
                            WHERE OrderID = {orderId}";
        }
        else
        {
            // For other status changes, just update the status
            updateQuery = $"UPDATE Orders SET Status = '{status}' WHERE OrderID = {orderId}";
        }

        // Execute the update query
        _db.ExecuteQuery(updateQuery);

        return RedirectToAction("OrderDetails", new { id = orderId });
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Error updating order status: {ex.Message}";
        return RedirectToAction("Orders");
    }
}

        // Add this new method to handle stock restoration when cancelling orders
        private void RestoreOrderStock(int orderId)
        {
            var query = $@"SELECT ProductID, Quantity 
                  FROM OrderItems 
                  WHERE OrderID = {orderId}";

            var orderItems = _db.SelectQuery(query);

            foreach (DataRow item in orderItems.Rows)
            {
                int productId = Convert.ToInt32(item["ProductID"]);
                int quantity = Convert.ToInt32(item["Quantity"]);

                // Add the quantity back to the product stock
                var restoreStockQuery = $@"UPDATE Products 
                                  SET Stock = Stock + {quantity} 
                                  WHERE ProductID = {productId}";
                _db.ExecuteQuery(restoreStockQuery);

                // Optional: Log the stock restoration for audit purposes
             //   LogStockMovement(productId, quantity, "RESTORED", $"Order #{orderId} cancelled");
            }
        }

        // Add this new method to handle stock reduction when reactivating cancelled orders
        private bool ReduceOrderStock(int orderId)
        {
            var query = $@"SELECT oi.ProductID, oi.Quantity, p.Stock 
                  FROM OrderItems oi
                  JOIN Products p ON oi.ProductID = p.ProductID
                  WHERE oi.OrderID = {orderId}";

            var orderItems = _db.SelectQuery(query);

            // First, check if we have enough stock for all items
            foreach (DataRow item in orderItems.Rows)
            {
                int currentStock = Convert.ToInt32(item["Stock"]);
                int requiredQuantity = Convert.ToInt32(item["Quantity"]);

                if (currentStock < requiredQuantity)
                {
                    return false; // Not enough stock
                }
            }

            // If we reach here, we have enough stock for all items, so reduce them
            foreach (DataRow item in orderItems.Rows)
            {
                int productId = Convert.ToInt32(item["ProductID"]);
                int quantity = Convert.ToInt32(item["Quantity"]);

                var reduceStockQuery = $@"UPDATE Products 
                                 SET Stock = Stock - {quantity} 
                                 WHERE ProductID = {productId}";
                _db.ExecuteQuery(reduceStockQuery);

                // Optional: Log the stock reduction for audit purposes
          //      LogStockMovement(productId, -quantity, "REDUCED", $"Order #{orderId} reactivated");
            }

            return true;
        }

       
        [HttpGet]
        public IActionResult ConfirmCancelOrder(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = GetOrderDetailsForAdmin(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Orders");
            }

            if (order.Status == "Cancelled")
            {
                TempData["Warning"] = "This order is already cancelled.";
                return RedirectToAction("OrderDetails", new { id });
            }

            if (order.Status == "Delivered")
            {
                TempData["Error"] = "Cannot cancel a delivered order.";
                return RedirectToAction("OrderDetails", new { id });
            }

            return View(order);
        }

        public IActionResult Categories()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var categories = GetCategories();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var query = $@"INSERT INTO Categories (CategoryName, Description) 
                              VALUES ('{category.CategoryName}', '{category.Description}')";

                var result = _db.ExecuteQuery(query);
                if (result > 0)
                {
                    TempData["Success"] = "Category created successfully!";
                    return RedirectToAction("Categories");
                }
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            try
            {
                // Use parameterized query to prevent SQL injection
                var query = "SELECT * FROM Categories WHERE CategoryId = @CategoryId";
                var parameters = new Dictionary<string, object>
        {
            { "@CategoryId", id }
        };

                var dt = _db.SelectQuery(query, parameters);

                if (dt.Rows.Count == 0)
                    return NotFound();

                var category = new Category
                {
                    CategoryID = Convert.ToInt32(dt.Rows[0]["CategoryId"]),
                    CategoryName = dt.Rows[0]["CategoryName"]?.ToString() ?? string.Empty,
                    Description = dt.Rows[0]["Description"]?.ToString() ?? string.Empty
                };

                return View(category);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading category.";
                return RedirectToAction("Categories");
            }
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                try
                {
                    // Use parameterized query to prevent SQL injection
                    var query = @"UPDATE Categories 
                         SET CategoryName = @CategoryName, Description = @Description 
                         WHERE CategoryId = @CategoryId";

                    var parameters = new Dictionary<string, object>
            {
                { "@CategoryName", category.CategoryName },
                { "@Description", category.Description },
                { "@CategoryId", category.CategoryID }
            };

                    var result = _db.ExecuteQuery(query, parameters);

                    if (result > 0)
                    {
                        TempData["Success"] = "Category updated successfully!";
                        return RedirectToAction("Categories");
                    }
                    else
                    {
                        TempData["Error"] = "No changes were made or category not found.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error updating category.";
                }
            }

            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var query = $"DELETE FROM Categories WHERE CategoryId = {id}";
            var result = _db.ExecuteQuery(query);

            if (result > 0)
                TempData["Success"] = "Category deleted successfully!";
            else
                TempData["Error"] = "Error deleting category.";

            return RedirectToAction("Categories");
        }

        private List<User> GetAllUsers()
        {
            var query = "SELECT * FROM Users ORDER BY CreatedAt DESC";
            var dt = _db.SelectQuery(query);
            var users = new List<User>();

            foreach (DataRow row in dt.Rows)
            {
                users.Add(new User
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    Firstname = row["Firstname"].ToString(),
                    Middlename = row["Middlename"].ToString(),
                    Lastname = row["Lastname"].ToString(),
                    Address = row["Address"].ToString(),
                    Role = row["Role"].ToString(),
                    Email = row["Email"].ToString(),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                });
            }

            return users;
        }

        private User GetUserById(int id)
        {
            var query = $"SELECT * FROM Users WHERE UserID = {id}";
            var dt = _db.SelectQuery(query);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new User
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Firstname = row["Firstname"].ToString(),
                Middlename = row["Middlename"].ToString(),
                Lastname = row["Lastname"].ToString(),
                Address = row["Address"].ToString(),
                Role = row["Role"].ToString(),
                Email = row["Email"].ToString(),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }

        private bool EmailExists(string email)
        {
            var query = $"SELECT COUNT(*) as Count FROM Users WHERE Email = '{email}'";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]) > 0;
        }

        private bool EmailExistsForOtherUser(string email, int userId)
        {
            var query = $"SELECT COUNT(*) as Count FROM Users WHERE Email = '{email}' AND UserID != {userId}";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]) > 0;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

     

        private List<Product> GetAllProducts()
        {
            var query = @"SELECT p.*, c.CategoryName 
                         FROM Products p 
                         LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                         ORDER BY p.CreatedAt DESC";

            var dt = _db.SelectQuery(query);
            var products = new List<Product>();

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    ProductName = row["ProductName"].ToString(),
                    Description = row["Description"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Stock = Convert.ToInt32(row["Stock"]),
                    CategoryID = row["CategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CategoryID"]),
                    Image = row["Image"].ToString(),
                    Visible = Convert.ToBoolean(row["Visible"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    CategoryName = row["CategoryName"]?.ToString()
                });
            }

            return products;
        }

        private Product GetProductById(int id)
        {
            var query = @"SELECT p.*, c.CategoryName 
                         FROM Products p 
                         LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                         WHERE p.ProductID = " + id;

            var dt = _db.SelectQuery(query);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new Product
            {
                ProductID = Convert.ToInt32(row["ProductID"]),
                ProductName = row["ProductName"].ToString(),
                Description = row["Description"].ToString(),
                Price = Convert.ToDecimal(row["Price"]),
                Stock = Convert.ToInt32(row["Stock"]),
                CategoryID = row["CategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["CategoryID"]),
                Image = row["Image"].ToString(),
                Visible = Convert.ToBoolean(row["Visible"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                CategoryName = row["CategoryName"]?.ToString()
            };
        }

        private List<Order> GetAllOrders()
        {
            var query = @"SELECT o.*, u.Firstname, u.Lastname 
                         FROM Orders o
                         JOIN Users u ON o.UserID = u.UserID
                         ORDER BY o.OrderDate DESC";

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

        private List<Category> GetCategories()
        {
            var query = "SELECT * FROM Categories ORDER BY CategoryName";
            var dt = _db.SelectQuery(query);
            var categories = new List<Category>();

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Category
                {
                    CategoryID = Convert.ToInt32(row["CategoryID"]),
                    CategoryName = row["CategoryName"].ToString(),
                    Description = row["Description"].ToString()
                });
            }

            return categories;
        }

        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = GetOrderDetailsForAdmin(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Orders");
            }

            return View(order);
        }

        private Order GetOrderDetailsForAdmin(int orderId)
        {
           
            var orderQuery = $@"SELECT o.*, u.Firstname, u.Lastname, u.Email, u.Address as UserAddress 
                       FROM Orders o
                       JOIN Users u ON o.UserID = u.UserID
                       WHERE o.OrderID = {orderId}";

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
                ShippingAddress = orderRow["ShippingAddress"].ToString(),
              
                CustomerName = $"{orderRow["Firstname"]} {orderRow["Lastname"]}",
                CustomerEmail = orderRow["Email"].ToString(),
                UserAddress = orderRow["UserAddress"].ToString()
            };

            var itemsQuery = $@"SELECT oi.*, p.ProductName, p.Description, p.Image 
                       FROM OrderItems oi
                       JOIN Products p ON oi.ProductID = p.ProductID
                       WHERE oi.OrderID = {orderId}";

            var itemsResult = _db.SelectQuery(itemsQuery);
            order.OrderItems = new List<OrderItem>();

            foreach (DataRow itemRow in itemsResult.Rows)
            {
                order.OrderItems.Add(new OrderItem
                {
                    OrderItemID = Convert.ToInt32(itemRow["OrderItemID"]),
                    OrderID = Convert.ToInt32(itemRow["OrderID"]),
                    ProductID = Convert.ToInt32(itemRow["ProductID"]),
                    Quantity = Convert.ToInt32(itemRow["Quantity"]),
                    PriceAtPurchase = Convert.ToDecimal(itemRow["PriceAtPurchase"]),
                    ProductName = itemRow["ProductName"].ToString(),
                    ProductDescription = itemRow["Description"].ToString(),
                    ProductImage = itemRow["Image"].ToString()
                });
            }

            return order;
        }
    }

}