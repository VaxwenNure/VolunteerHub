using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Mvc.Models;
using VolunteerHub.Mvc.Services;

namespace VolunteerHub.Mvc.Controllers
{
    public class HelpTypesController : Controller
    {
        private readonly IJsonStore _store;

        public HelpTypesController(IJsonStore store) => _store = store;

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

            var items = _store.GetHelpTypes().OrderBy(x => x.Id).ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!EnsureLogin()) return new EmptyResult();
            return View(new HelpType());
        }

        [HttpPost]
        public IActionResult Create(HelpType model)
        {
            if (!EnsureLogin()) return new EmptyResult();

            model.Name = (model.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Required");
                return View(model);
            }

            var list = _store.GetHelpTypes();
            model.Id = list.Count == 0 ? 1 : list.Max(x => x.Id) + 1;

            list.Add(model);
            _store.SaveHelpTypes(list);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var item = _store.GetHelpTypes().FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(HelpType model)
        {
            if (!EnsureLogin()) return new EmptyResult();

            model.Name = (model.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Required");
                return View(model);
            }

            var list = _store.GetHelpTypes();
            var item = list.FirstOrDefault(x => x.Id == model.Id);
            if (item == null) return NotFound();

            item.Name = model.Name;
            _store.SaveHelpTypes(list);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            var item = _store.GetHelpTypes().FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!EnsureLogin()) return new EmptyResult();

            // optional safety: do not delete if used in help records
            var used = _store.GetHelpRecords().Any(r => r.HelpTypeId == id);
            if (used)
            {
                TempData["Error"] = "Cannot delete: this help type is used in help records.";
                return RedirectToAction(nameof(Index));
            }

            var list = _store.GetHelpTypes();
            list.RemoveAll(x => x.Id == id);
            _store.SaveHelpTypes(list);

            return RedirectToAction(nameof(Index));
        }
    }
}
