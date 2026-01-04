using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VolunteerHub.Mvc.Models;
using VolunteerHub.Mvc.Models.ViewModels;
using VolunteerHub.Mvc.Services;

namespace VolunteerHub.Mvc.Controllers
{
    public class HelpRecordsController : Controller
    {
        private readonly IJsonStore _store;
        public HelpRecordsController(IJsonStore store) => _store = store;

        private bool EnsureLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                Response.Redirect("/Session/Login");
                return false;
            }
            return true;
        }

        public IActionResult Index()
        {
            if (!EnsureLogin()) return new EmptyResult();

            var records = _store.GetHelpRecords().OrderByDescending(x => x.Date).ToList();
            var volunteers = _store.GetVolunteers();
            var types = _store.GetHelpTypes();

            // simple join for display (without DB)
            ViewBag.Volunteers = volunteers.ToDictionary(v => v.Id, v => v.FullName);
            ViewBag.HelpTypes = types.ToDictionary(t => t.Id, t => t.Name);

            return View(records);
        }

        private HelpRecordEditVm BuildVm(HelpRecord? r = null)
        {
            var volunteers = _store.GetVolunteers().OrderBy(v => v.FullName)
                .Select(v => new SelectListItem(v.FullName, v.Id.ToString()))
                .ToList();

            var types = _store.GetHelpTypes().OrderBy(t => t.Name)
                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                .ToList();

            return new HelpRecordEditVm
            {
                Id = r?.Id ?? 0,
                VolunteerId = r?.VolunteerId ?? (volunteers.Count > 0 ? int.Parse(volunteers[0].Value) : 0),
                HelpTypeId = r?.HelpTypeId ?? (types.Count > 0 ? int.Parse(types[0].Value) : 0),
                Date = r?.Date ?? DateTime.Today,
                Amount = r?.Amount ?? 0,
                Description = r?.Description ?? "",
                Volunteers = volunteers,
                HelpTypes = types
            };
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!EnsureLogin()) return new EmptyResult();
            return View(BuildVm());
        }

        [HttpPost]
        public IActionResult Create(HelpRecordEditVm vm)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var list = _store.GetHelpRecords();
            var id = list.Count == 0 ? 1 : list.Max(x => x.Id) + 1;

            list.Add(new HelpRecord
            {
                Id = id,
                VolunteerId = vm.VolunteerId,
                HelpTypeId = vm.HelpTypeId,
                Date = vm.Date,
                Amount = vm.Amount,
                Description = vm.Description ?? ""
            });

            _store.SaveHelpRecords(list);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var rec = _store.GetHelpRecords().FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();
            return View(BuildVm(rec));
        }

        [HttpPost]
        public IActionResult Edit(HelpRecordEditVm vm)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var list = _store.GetHelpRecords();
            var rec = list.FirstOrDefault(x => x.Id == vm.Id);
            if (rec == null) return NotFound();

            rec.VolunteerId = vm.VolunteerId;
            rec.HelpTypeId = vm.HelpTypeId;
            rec.Date = vm.Date;
            rec.Amount = vm.Amount;
            rec.Description = vm.Description ?? "";

            _store.SaveHelpRecords(list);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();
            var rec = _store.GetHelpRecords().FirstOrDefault(x => x.Id == id);
            if (rec == null) return NotFound();
            return View(rec);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var list = _store.GetHelpRecords();
            list.RemoveAll(x => x.Id == id);
            _store.SaveHelpRecords(list);

            return RedirectToAction(nameof(Index));
        }
    }
}
