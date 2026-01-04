using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Mvc.Services;

namespace VolunteerHub.Mvc.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportService _report;
        public ReportsController(ReportService report) => _report = report;

        private bool EnsureLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                Response.Redirect("/Session/Login");
                return false;
            }
            return true;
        }

        // /Reports/TotalsByVolunteer?from=2026-01-01&to=2026-01-31
        public IActionResult TotalsByVolunteer(DateTime? from, DateTime? to)
        {
            if (!EnsureLogin()) return new EmptyResult();

            ViewBag.From = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.To = to?.ToString("yyyy-MM-dd") ?? "";

            var data = _report.TotalsByVolunteer(from, to);
            return View(data);
        }

        // /Reports/TotalsByHelpType?from=...&to=...
        public IActionResult TotalsByHelpType(DateTime? from, DateTime? to)
        {
            if (!EnsureLogin()) return new EmptyResult();

            ViewBag.From = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.To = to?.ToString("yyyy-MM-dd") ?? "";

            var data = _report.TotalsByHelpType(from, to);
            return View(data);
        }
    }
}
