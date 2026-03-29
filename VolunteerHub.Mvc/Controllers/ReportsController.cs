using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Mvc.Services;

namespace VolunteerHub.Mvc.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportService _report;
        public ReportsController(ReportService report) => _report = report;

        public IActionResult TotalsByVolunteer(DateTime? from, DateTime? to)
        {
            ViewBag.From = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.To = to?.ToString("yyyy-MM-dd") ?? "";
            return View(_report.TotalsByVolunteer(from, to));
        }

        public IActionResult TotalsByHelpType(DateTime? from, DateTime? to)
        {
            ViewBag.From = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.To = to?.ToString("yyyy-MM-dd") ?? "";
            return View(_report.TotalsByHelpType(from, to));
        }
    }
}
