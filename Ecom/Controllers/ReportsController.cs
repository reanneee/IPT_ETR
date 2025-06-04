using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Ecom.Models;

namespace YourApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly string _connectionString;

        public ReportsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Reports
        public IActionResult Index()
        {
            return View();
        }

        // GET: Reports/SalesSummary
        public async Task<IActionResult> SalesSummary(DateTime? startDate, DateTime? endDate)
        {
            // Set default date range if not provided
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var salesData = await GetSalesSummary(startDate.Value, endDate.Value);

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View(salesData);
        }

        // GET: Reports/TopSelling
        public async Task<IActionResult> TopSelling(DateTime? startDate, DateTime? endDate, int limit = 10)
        {
            // Set default date range if not provided
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            var topSellingData = await GetTopSellingProducts(startDate.Value, endDate.Value, limit);

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.Limit = limit;

            return View(topSellingData);
        }


        private async Task<SalesSummaryModel> GetSalesSummary(DateTime startDate, DateTime endDate)
        {
            var summary = new SalesSummaryModel();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get total sales
                var totalSalesQuery = @"
                    SELECT 
                        COUNT(*) as TotalOrders,
                        COALESCE(SUM(TotalAmount), 0) as TotalRevenue,
                        COALESCE(AVG(TotalAmount), 0) as AverageOrderValue
                    FROM orders 
                    WHERE OrderDate >= @startDate AND OrderDate <= @endDate
                    AND Status != 'Cancelled'";

                using (var cmd = new MySqlCommand(totalSalesQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            summary.TotalOrders = reader.GetInt32("TotalOrders");
                            summary.TotalRevenue = reader.GetDecimal("TotalRevenue");
                            summary.AverageOrderValue = reader.GetDecimal("AverageOrderValue");
                        }
                    }
                }

                // Get daily sales breakdown
                var dailySalesQuery = @"
                    SELECT 
                        DATE(OrderDate) as SaleDate,
                        COUNT(*) as OrderCount,
                        COALESCE(SUM(TotalAmount), 0) as DailyRevenue
                    FROM orders 
                    WHERE OrderDate >= @startDate AND OrderDate <= @endDate
                    AND Status != 'Cancelled'
                    GROUP BY DATE(OrderDate)
                    ORDER BY SaleDate";

                using (var cmd = new MySqlCommand(dailySalesQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        summary.DailySales = new List<DailySalesModel>();
                        while (await reader.ReadAsync())
                        {
                            summary.DailySales.Add(new DailySalesModel
                            {
                                Date = reader.GetDateTime("SaleDate"),
                                OrderCount = reader.GetInt32("OrderCount"),
                                Revenue = reader.GetDecimal("DailyRevenue")
                            });
                        }
                    }
                }
            }

            summary.StartDate = startDate;
            summary.EndDate = endDate;
            return summary;
        }

        private async Task<List<TopSellingProductModel>> GetTopSellingProducts(DateTime startDate, DateTime endDate, int limit)
        {
            var topProducts = new List<TopSellingProductModel>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        p.ProductID,
                        p.ProductName,
                        p.Price,
                        c.CategoryName,
                        SUM(oi.Quantity) as TotalQuantitySold,
                        SUM(oi.Quantity * oi.PriceAtPurchase) as TotalRevenue,
                        COUNT(DISTINCT o.OrderID) as NumberOfOrders
                    FROM products p
                    INNER JOIN orderitems oi ON p.ProductID = oi.ProductID
                    INNER JOIN orders o ON oi.OrderID = o.OrderID
                    LEFT JOIN categories c ON p.CategoryID = c.CategoryID
                    WHERE o.OrderDate >= @startDate AND o.OrderDate <= @endDate
                    AND o.Status != 'Cancelled'
                    GROUP BY p.ProductID, p.ProductName, p.Price, c.CategoryName
                    ORDER BY TotalQuantitySold DESC
                    LIMIT @limit";

                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@limit", limit);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            topProducts.Add(new TopSellingProductModel
                            {
                                ProductId = reader.GetInt32("ProductID"),
                                ProductName = reader.GetString("ProductName"),
                                Price = reader.GetDecimal("Price"),
                                CategoryName = reader.IsDBNull("CategoryName") ? "No Category" : reader.GetString("CategoryName"),
                                TotalQuantitySold = reader.GetInt32("TotalQuantitySold"),
                                TotalRevenue = reader.GetDecimal("TotalRevenue"),
                                NumberOfOrders = reader.GetInt32("NumberOfOrders")
                            });
                        }
                    }
                }
            }

            return topProducts;
        }
    }

  
   
}