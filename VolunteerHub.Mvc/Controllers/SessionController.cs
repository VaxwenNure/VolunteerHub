using Microsoft.AspNetCore.Mvc;

namespace VolunteerHub.Mvc.Controllers
{
    public class SessionController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string userName)
        {
            userName = (userName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(userName))
            {
                ViewBag.Error = "Enter name";
                return View();
            }

            HttpContext.Session.SetString("user", userName);
            return RedirectToAction("Index", "Volunteers");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("user");
            return RedirectToAction("Login");
        }

        private bool EnsureLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                Response.Redirect("/Session/Login");
                return false;
            }
            return true;
        }
    }
}
