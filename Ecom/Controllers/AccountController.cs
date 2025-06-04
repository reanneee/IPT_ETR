using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ecom.DataAccess;
using Ecom.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Ecom.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _db;

        public AccountController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashPassword(model.Password);
                var query = $"SELECT * FROM Users WHERE Email = '{model.Email}' AND Password = '{hashedPassword}'";
                var dt = _db.SelectQuery(query);
                if (dt.Rows.Count > 0)
                {
                    var user = dt.Rows[0];
                    HttpContext.Session.SetInt32("UserID", Convert.ToInt32(user["UserID"]));
                    HttpContext.Session.SetString("UserName", user["Firstname"].ToString() + " " + user["Lastname"].ToString());
                    HttpContext.Session.SetString("UserRole", user["Role"].ToString());

                    // Transfer guest cart to user cart
                    TransferGuestCartToUser(Convert.ToInt32(user["UserID"]));

                    // Check user role and redirect accordingly
                    string userRole = user["Role"].ToString();
                    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                var checkQuery = $"SELECT COUNT(*) as Count FROM Users WHERE Email = '{model.Email}'";
                var checkResult = _db.SelectQuery(checkQuery);

                if (Convert.ToInt32(checkResult.Rows[0]["Count"]) > 0)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                var hashedPassword = HashPassword(model.Password);
                var query = $@"INSERT INTO Users (Firstname, Middlename, Lastname, Address, Email, Password) 
                              VALUES ('{model.Firstname}', '{model.Middlename}', '{model.Lastname}', 
                                     '{model.Address}', '{model.Email}', '{hashedPassword}')";

                var result = _db.ExecuteQuery(query);

                if (result > 0)
                {
                    TempData["Success"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void TransferGuestCartToUser(int userId)
        {
            var sessionId = HttpContext.Session.Id;

            // Get or create user cart
            var cartQuery = $"SELECT CartID FROM Cart WHERE UserID = {userId}";
            var cartResult = _db.SelectQuery(cartQuery);

            int cartId;
            if (cartResult.Rows.Count == 0)
            {
                var createCartQuery = $"INSERT INTO Cart (UserID) VALUES ({userId})";
                _db.ExecuteQuery(createCartQuery);

                var getCartIdQuery = $"SELECT LAST_INSERT_ID() as CartID";
                var cartIdResult = _db.SelectQuery(getCartIdQuery);
                cartId = Convert.ToInt32(cartIdResult.Rows[0]["CartID"]);
            }
            else
            {
                cartId = Convert.ToInt32(cartResult.Rows[0]["CartID"]);
            }

            // Transfer guest cart items
            var guestItemsQuery = $"SELECT * FROM GuestCart WHERE SessionID = '{sessionId}'";
            var guestItems = _db.SelectQuery(guestItemsQuery);

            foreach (DataRow item in guestItems.Rows)
            {
                var productId = Convert.ToInt32(item["ProductID"]);
                var quantity = Convert.ToInt32(item["Quantity"]);

                var insertQuery = $@"INSERT INTO CartItems (CartID, ProductID, Quantity) 
                                    VALUES ({cartId}, {productId}, {quantity})
                                    ON DUPLICATE KEY UPDATE Quantity = Quantity + {quantity}";
                _db.ExecuteQuery(insertQuery);
            }

            // Clear guest cart
            var clearGuestCartQuery = $"DELETE FROM GuestCart WHERE SessionID = '{sessionId}'";
            _db.ExecuteQuery(clearGuestCartQuery);
        }
    }
}
