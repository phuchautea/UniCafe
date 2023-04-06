using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StatsController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Admin/Stats
        public ActionResult Index()
        {

            var currentYear = DateTime.Now.Year;
            var sales = _context.Orders
                .Where(o => o.CreatedAt.Year == currentYear)
                .GroupBy(o => new { Year = o.CreatedAt.Year, Month = o.CreatedAt.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Sales = g.Sum(o => o.Total),
                })
                .OrderBy(g => g.Month)
                .ToList();

            var salesStatistics = new List<object[]>();
            salesStatistics.Add(new object[] { "Tháng", "Doanh thu" });
            foreach (var data in sales)
            {
                salesStatistics.Add(new object[] { "Tháng "+data.Month, (int)data.Sales });
            }

            var payment_methods = _context.Orders
                .GroupBy(o => o.Payment)
                .Select(g => new
                {
                    PaymentMethod = g.Key,
                    Total = g.Count(),
                })
                .ToList();

            var paymentMethodStatistics = new List<object[]>();
            paymentMethodStatistics.Add(new object[] { "Phương thức thanh toán", "Tổng" });
            foreach (var data in payment_methods)
            {
                paymentMethodStatistics.Add(new object[] { data.PaymentMethod, data.Total });
            }

            ViewBag.salesStatistics = salesStatistics.ToArray();
            ViewBag.paymentMethodStatistics = paymentMethodStatistics.ToArray();

            return View();
        }
    }
}