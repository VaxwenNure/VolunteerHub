using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Mvc.Models;
using VolunteerHub.Mvc.Services;
namespace VolunteerHub.Mvc.Controllers
{
    public class VolunteersController : Controller
    {
        private readonly IJsonStore _store;

        public VolunteersController(IJsonStore store) => _store = store;

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

            var items = _store.GetVolunteers().OrderBy(x => x.Id).ToList();
            return View(items);
        }

        public IActionResult Details(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var item = _store.GetVolunteers().FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!EnsureLogin()) return new EmptyResult();
            return View(new Volunteer());
        }

        [HttpPost]
        public IActionResult Create(Volunteer model)
        {
            if (!EnsureLogin()) return new EmptyResult();

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError(nameof(model.FullName), "Required");
                return View(model);
            }

            var list = _store.GetVolunteers();
            model.Id = list.Count == 0 ? 1 : list.Max(x => x.Id) + 1;
            list.Add(model);
            _store.SaveVolunteers(list);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var item = _store.GetVolunteers().FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Volunteer model)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var list = _store.GetVolunteers();
            var item = list.FirstOrDefault(x => x.Id == model.Id);
            if (item == null) return NotFound();

            item.FullName = model.FullName.Trim();
            item.Phone = (model.Phone ?? "").Trim();

            _store.SaveVolunteers(list);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var item = _store.GetVolunteers().FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var list = _store.GetVolunteers();
            list.RemoveAll(x => x.Id == id);
            _store.SaveVolunteers(list);

            return RedirectToAction(nameof(Index));
        }
    }

}
