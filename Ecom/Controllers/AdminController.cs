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
using System.Xml;
using System.Xml.Schema;

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

            return View();
        }

        // XML Upload Feature
        [HttpGet]
        public IActionResult UploadXML()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadXML(IFormFile file, bool importData = false)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file to upload.";
                ViewBag.MessageType = "error";
                return View();
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xml")
            {
                ViewBag.Message = "Invalid file type. Please select a valid XML file.";
                ViewBag.MessageType = "error";
                return View();
            }

            try
            {
                string schemaPath = Path.Combine(_webHostEnvironment.WebRootPath, "Schemas", "Products.xsd");

                // Check if schema file exists
                if (!System.IO.File.Exists(schemaPath))
                {
                    ViewBag.Message = "Schema file not found. Please ensure Products.xsd exists in wwwroot/Schemas folder.";
                    ViewBag.MessageType = "error";
                    return View();
                }

                bool isValid = ValidateXml(file, schemaPath, out string validationMessage);

                if (isValid)
                {
                    if (importData)
                    {
                        // Parse and import the XML data
                        var importResult = await ImportProductsFromXml(file);
                        ViewBag.Message = $"XML file is valid and data imported successfully! {importResult.ImportedCount} products imported, {importResult.SkippedCount} skipped.";
                        ViewBag.MessageType = "success";
                        ViewBag.ImportDetails = importResult.Details;
                    }
                    else
                    {
                        ViewBag.Message = "XML file is valid and ready for import.";
                        ViewBag.MessageType = "success";
                        ViewBag.ShowImportButton = true;
                    }
                }
                else
                {
                    ViewBag.Message = $"XML file is invalid. Error: {validationMessage}";
                    ViewBag.MessageType = "error";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred while processing the file: {ex.Message}";
                ViewBag.MessageType = "error";
            }

            return View();
        }

        private bool ValidateXml(IFormFile file, string xsdPath, out string validationMessage)
        {
            validationMessage = string.Empty;
            bool isValid = true;
            List<string> errors = new List<string>();

            try
            {
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("", xsdPath);

                using (var stream = file.OpenReadStream())
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        Schemas = schemaSet,
                        ValidationType = ValidationType.Schema
                    };

                    settings.ValidationEventHandler += (sender, args) =>
                    {
                        isValid = false;
                        errors.Add($"Line {args.Exception?.LineNumber}: {args.Message}");
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        while (reader.Read()) { }
                    }
                }

                if (errors.Any())
                {
                    validationMessage = string.Join("; ", errors);
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                validationMessage = ex.Message;
            }

            return isValid;
        }

        private async Task<ImportResult> ImportProductsFromXml(IFormFile file)
        {
            var result = new ImportResult();

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(stream);

                    XmlNodeList productNodes = doc.SelectNodes("//Product");

                    foreach (XmlNode productNode in productNodes)
                    {
                        try
                        {
                            var product = ParseProductFromXml(productNode);

                            // Check if product already exists
                            if (ProductExists(product.ProductName))
                            {
                                result.SkippedCount++;
                                result.Details.Add($"Skipped: {product.ProductName} (already exists)");
                                continue;
                            }

                            // Validate category exists
                            if (product.CategoryID.HasValue && !CategoryExists(product.CategoryID.Value))
                            {
                                result.SkippedCount++;
                                result.Details.Add($"Skipped: {product.ProductName} (invalid category ID: {product.CategoryID})");
                                continue;
                            }

                            // Insert product
                            if (InsertProduct(product))
                            {
                                result.ImportedCount++;
                                result.Details.Add($"Imported: {product.ProductName}");
                            }
                            else
                            {
                                result.SkippedCount++;
                                result.Details.Add($"Failed to import: {product.ProductName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            result.SkippedCount++;
                            result.Details.Add($"Error processing product: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Details.Add($"Error reading XML file: {ex.Message}");
            }

            return result;
        }

        private Product ParseProductFromXml(XmlNode productNode)
        {
            return new Product
            {
                ProductName = GetXmlNodeValue(productNode, "ProductName"),
                Description = GetXmlNodeValue(productNode, "Description"),
                Price = decimal.Parse(GetXmlNodeValue(productNode, "Price")),
                Stock = int.Parse(GetXmlNodeValue(productNode, "Stock")),
                CategoryID = string.IsNullOrEmpty(GetXmlNodeValue(productNode, "CategoryID")) ?
                            (int?)null : int.Parse(GetXmlNodeValue(productNode, "CategoryID")),
                Image = GetXmlNodeValue(productNode, "Image"),
                Visible = bool.Parse(GetXmlNodeValue(productNode, "Visible")),
                CreatedAt = DateTime.Now
            };
        }

        private string GetXmlNodeValue(XmlNode parentNode, string nodeName)
        {
            return parentNode.SelectSingleNode(nodeName)?.InnerText ?? string.Empty;
        }

        private bool ProductExists(string productName)
        {
            var query = $"SELECT COUNT(*) as Count FROM Products WHERE ProductName = '{productName.Replace("'", "''")}'";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]) > 0;
        }

        private bool CategoryExists(int categoryId)
        {
            var query = $"SELECT COUNT(*) as Count FROM Categories WHERE CategoryID = {categoryId}";
            var result = _db.SelectQuery(query);
            return Convert.ToInt32(result.Rows[0]["Count"]) > 0;
        }

        private bool InsertProduct(Product product)
        {
            try
            {
                var categoryValue = product.CategoryID?.ToString() ?? "NULL";
                var query = $@"INSERT INTO Products (ProductName, Description, Price, Stock, CategoryID, Image, Visible, CreatedAt) 
                              VALUES ('{product.ProductName.Replace("'", "''")}', 
                                     '{product.Description.Replace("'", "''")}', 
                                     {product.Price}, 
                                     {product.Stock}, 
                                     {categoryValue}, 
                                     '{product.Image.Replace("'", "''")}', 
                                     {(product.Visible ? 1 : 0)}, 
                                     '{DateTime.Now:yyyy-MM-dd HH:mm:ss}')";

                var result = _db.ExecuteQuery(query);
                return result > 0;
            }
            catch
            {
                return false;
            }
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

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var query = $"UPDATE Orders SET Status = '{status}' WHERE OrderID = {orderId}";
            _db.ExecuteQuery(query);

            TempData["Success"] = "Order status updated successfully!";
            return RedirectToAction("Orders");
        }

        // Category Management
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

        // User Helper Methods
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

        // Other Helper Methods
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
    }

    // Helper class for import results
    public class ImportResult
    {
        public int ImportedCount { get; set; } = 0;
        public int SkippedCount { get; set; } = 0;
        public List<string> Details { get; set; } = new List<string>();
    }
}