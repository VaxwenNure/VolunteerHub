using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteerHub.Data.Data;
using VolunteerHub.Data.Models;
using VolunteerHub.Mvc.Models.ViewModels;

namespace VolunteerHub.Mvc.Controllers
{
    public class HelpRecordsController : Controller
    {
        private readonly VolunteerHubContext _db;
        public HelpRecordsController(VolunteerHubContext db) => _db = db;

        public IActionResult Index()
        {
            var records = _db.VolunteerHelps
                             .Include(h => h.Volunteer)
                             .Include(h => h.HelpType)
                             .OrderByDescending(h => h.HelpDate)
                             .ToList();
            return View(records);
        }

        private HelpRecordEditVm BuildVm(VolunteerHelp? r = null)
        {
            var volunteers = _db.Volunteers.OrderBy(v => v.FullName)
                .Select(v => new SelectListItem(v.FullName, v.VolunteerId.ToString()))
                .ToList();

            var types = _db.HelpTypes.OrderBy(t => t.Name)
                .Select(t => new SelectListItem(t.Name, t.HelpTypeId.ToString()))
                .ToList();

            return new HelpRecordEditVm
            {
                VolunteerHelpId = r?.VolunteerHelpId ?? 0,
                VolunteerId = r?.VolunteerId ?? (volunteers.Count > 0 ? int.Parse(volunteers[0].Value) : 0),
                HelpTypeId = r?.HelpTypeId ?? (types.Count > 0 ? int.Parse(types[0].Value) : 0),
                HelpDate = r?.HelpDate ?? DateTime.Today,
                Amount = r?.Amount ?? 0,
                Description = r?.Description ?? "",
                Volunteers = volunteers,
                HelpTypes = types
            };
        }

        [HttpGet]
        public IActionResult Create() => View(BuildVm());

        [HttpPost]
        public IActionResult Create(HelpRecordEditVm vm)
        {
            _db.VolunteerHelps.Add(new VolunteerHelp
            {
                VolunteerId = vm.VolunteerId,
                HelpTypeId = vm.HelpTypeId,
                HelpDate = vm.HelpDate,
                Amount = vm.Amount,
                Description = vm.Description ?? ""
            });
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var rec = _db.VolunteerHelps.Find(id);
            if (rec == null) return NotFound();
            return View(BuildVm(rec));
        }

        [HttpPost]
        public IActionResult Edit(HelpRecordEditVm vm)
        {
            var rec = _db.VolunteerHelps.Find(vm.VolunteerHelpId);
            if (rec == null) return NotFound();

            rec.VolunteerId = vm.VolunteerId;
            rec.HelpTypeId = vm.HelpTypeId;
            rec.HelpDate = vm.HelpDate;
            rec.Amount = vm.Amount;
            rec.Description = vm.Description ?? "";

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var rec = _db.VolunteerHelps
                         .Include(h => h.Volunteer)
                         .Include(h => h.HelpType)
                         .FirstOrDefault(h => h.VolunteerHelpId == id);
            if (rec == null) return NotFound();
            return View(rec);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var rec = _db.VolunteerHelps.Find(id);
            if (rec != null)
            {
                _db.VolunteerHelps.Remove(rec);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
