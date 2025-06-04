using Ecom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ecom.DataAccess;
using System.Data;

namespace Ecom.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _db;

        public HomeController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var products = GetFeaturedProducts();
            return View(products);
        }

        private List<Product> GetFeaturedProducts()
        {
            var query = @"SELECT p.*, c.CategoryName 
                         FROM Products p 
                         LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                         WHERE p.Visible = TRUE 
                         ORDER BY p.CreatedAt DESC 
                         LIMIT 8";

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
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            return products;
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
