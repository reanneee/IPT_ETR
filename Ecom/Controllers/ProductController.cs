using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Ecom.DataAccess;
using Ecom.Models;
using System.Data;

namespace Ecom.Controllers
{
    public class ProductController : Controller
    {
        private readonly DatabaseHelper _db;

        public ProductController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index(int? categoryId, string search)
        {
            var products = GetProducts(categoryId, search);
            ViewBag.Categories = GetCategories();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        private List<Product> GetProducts(int? categoryId, string search)
        {
            var query = @"SELECT p.*, c.CategoryName 
                         FROM Products p 
                         LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                         WHERE p.Visible = TRUE";

            if (categoryId.HasValue)
            {
                query += $" AND p.CategoryID = {categoryId.Value}";
            }

            if (!string.IsNullOrEmpty(search))
            {
                query += $" AND (p.ProductName LIKE '%{search}%' OR p.Description LIKE '%{search}%')";
            }

            query += " ORDER BY p.ProductName";

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

        private Product GetProductById(int id)
        {
            var query = @"SELECT p.*, c.CategoryName 
                         FROM Products p 
                         LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                         WHERE p.ProductID = " + id + " AND p.Visible = TRUE";

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
                CategoryName = row["CategoryName"].ToString()
            };
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
}

