using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerHub.Data.Data;
using VolunteerHub.Mvc.Models.ViewModels;

namespace VolunteerHub.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly VolunteerHubContext _db;
        public HomeController(VolunteerHubContext db) => _db = db;

        public IActionResult Index()
        {
            var vm = new DashboardVm
            {
                TotalVolunteers  = _db.Volunteers.Count(),
                TotalHelpTypes   = _db.HelpTypes.Count(),
                TotalRecords     = _db.VolunteerHelps.Count(),
                TotalAmount      = _db.VolunteerHelps.Sum(h => (decimal?)h.Amount) ?? 0,
                RecentRecords    = _db.VolunteerHelps
                                      .Include(h => h.Volunteer)
                                      .Include(h => h.HelpType)
                                      .OrderByDescending(h => h.HelpDate)
                                      .Take(5)
                                      .ToList()
            };
            return View(vm);
        }
    }
}
