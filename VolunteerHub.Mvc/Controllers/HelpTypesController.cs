using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Data.Data;
using VolunteerHub.Data.Models;

namespace VolunteerHub.Mvc.Controllers
{
    public class HelpTypesController : Controller
    {
        private readonly VolunteerHubContext _db;
        public HelpTypesController(VolunteerHubContext db) => _db = db;

        public IActionResult Index()
        {
            var items = _db.HelpTypes.OrderBy(x => x.Name).ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create() => View(new HelpType());

        [HttpPost]
        public IActionResult Create(HelpType model)
        {
            model.Name = (model.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Required");
                return View(model);
            }
            _db.HelpTypes.Add(model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _db.HelpTypes.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(HelpType model)
        {
            model.Name = (model.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Required");
                return View(model);
            }
            var item = _db.HelpTypes.Find(model.HelpTypeId);
            if (item == null) return NotFound();
            item.Name = model.Name;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _db.HelpTypes.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // Safety: do not delete if used in help records
            var used = _db.VolunteerHelps.Any(r => r.HelpTypeId == id);
            if (used)
            {
                TempData["Error"] = "Cannot delete: this help type is used in help records.";
                return RedirectToAction(nameof(Index));
            }
            var item = _db.HelpTypes.Find(id);
            if (item != null)
            {
                _db.HelpTypes.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
