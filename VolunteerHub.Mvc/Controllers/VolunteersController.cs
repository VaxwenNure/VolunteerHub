using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerHub.Data.Data;
using VolunteerHub.Data.Models;

namespace VolunteerHub.Mvc.Controllers
{
    public class VolunteersController : Controller
    {
        private readonly VolunteerHubContext _db;
        public VolunteersController(VolunteerHubContext db) => _db = db;

        public IActionResult Index()
        {
            var items = _db.Volunteers.OrderBy(x => x.FullName).ToList();
            return View(items);
        }

        public IActionResult Details(int id)
        {
            var item = _db.Volunteers
                          .Include(v => v.VolunteerHelps)
                              .ThenInclude(h => h.HelpType)
                          .FirstOrDefault(x => x.VolunteerId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create() => View(new Volunteer());

        [HttpPost]
        public IActionResult Create(Volunteer model)
        {
            model.FullName = (model.FullName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError(nameof(model.FullName), "Required");
                return View(model);
            }
            _db.Volunteers.Add(model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _db.Volunteers.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Volunteer model)
        {
            model.FullName = (model.FullName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError(nameof(model.FullName), "Required");
                return View(model);
            }
            var item = _db.Volunteers.Find(model.VolunteerId);
            if (item == null) return NotFound();
            item.FullName = model.FullName;
            item.Phone = (model.Phone ?? "").Trim();
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _db.Volunteers.Find(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _db.Volunteers.Find(id);
            if (item != null)
            {
                _db.Volunteers.Remove(item);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
