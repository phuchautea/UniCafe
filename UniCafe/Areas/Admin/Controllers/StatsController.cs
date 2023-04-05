using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniCafe.Models;

namespace UniCafe.Areas.Admin.Controllers
{
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
            Dictionary<string, string> monthMap = new Dictionary<string, string>()
{
    { "January", "Tháng 1" },
    { "February", "Tháng 2" },
    { "March", "Tháng 3" },
    { "April", "Tháng 4" },
    { "May", "Tháng 5" },
    { "June", "Tháng 6" },
    { "July", "Tháng 7" },
    { "August", "Tháng 8" },
    { "September", "Tháng 9" },
    { "October", "Tháng 10" },
    { "November", "Tháng 11" },
    { "December", "Tháng 12" },
};

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
                //var monthName = monthMap.FirstOrDefault(m => m.Value == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(data.Month)).Key;
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

            //var bestSellingProducts = _context.Products
            //    .OrderByDescending(p => p.sold_quantity)
            //    .Take(5)
            //    .Select(p => new
            //    {
            //        p.id,
            //        p.name,
            //        p.sold_quantity,
            //    })
            //    .ToList();

            //var soldQuantityStatistics = new List<object[]>();
            //soldQuantityStatistics.Add(new object[] { "Tên", "Số lượng" });
            //foreach (var data in bestSellingProducts)
            //{
            //    soldQuantityStatistics.Add(new object[] { data.name, data.sold_quantity });
            //}
            ViewBag.salesStatistics = salesStatistics.ToArray();
            ViewBag.paymentMethodStatistics = paymentMethodStatistics.ToArray();
            //return View(new
            //{
            //    salesStatistics = salesStatistics.ToArray(),
            //    paymentMethodStatistics = paymentMethodStatistics.ToArray(),
            //    //soldQuantityStatistics = soldQuantityStatistics.ToArray(),
            //});

            return View();
        }
    }
}